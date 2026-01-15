using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using CustomerCampaignService.Application.Contracts.Purchases;
using CustomerCampaignService.Application.Contracts.Customers;
using CustomerCampaignService.Application.Errors;
using CustomerCampaignService.Application.Interfaces;
using CustomerCampaignService.Domain.Entities;

namespace CustomerCampaignService.Application.Services;

public sealed class PurchaseImportService : IPurchaseImportService
{
    private static readonly string[] RequiredHeaders =
    {
        "CustomerId",
        "TransactionId",
        "PurchaseDate",
        "Amount"
    };

    private readonly IPurchaseImportRepository _repository;
    private readonly ICustomerLookupService _customerLookupService;

    public PurchaseImportService(IPurchaseImportRepository repository,ICustomerLookupService customerLookupService)
    {
        _repository = repository;
        _customerLookupService = customerLookupService;
    }

    public async Task<PurchaseImportResult> ImportFileAsync(int campaignCode, string fileName, Stream csvStream, CancellationToken cancellationToken)
    {
        var campaign = await _repository.GetCampaignByCodeAsync(campaignCode, cancellationToken);
        if (campaign is null)
        {
            throw new CCSException(404, "Campaign not found", $"CampaignCode '{campaignCode}' does not exist.");
        }

        using var memory = new MemoryStream();
        
        await csvStream.CopyToAsync(memory, cancellationToken);
       
        var fileHash = ComputeSha256(memory.ToArray());
        memory.Position = 0;

        using var reader = new StreamReader(memory, Encoding.UTF8, true, leaveOpen: true);
        
        var headerLine = await reader.ReadLineAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(headerLine))
        {
            throw new CCSException(400, "Invalid CSV", "Header row is missing.");
        }

        var headers = SplitCsvLine(headerLine);
        var headerMap = BuildHeaderMap(headers);
        EnsureRequiredHeaders(headerMap);

        var batch = new PurchaseImport
        {
            ImportBatchId = Guid.NewGuid(),
            CampaignId = campaign.CampaignId,
            FileName = fileName,
            FileHash = fileHash,
            Status = ImportBatchStatus.Received,
            ImportedAt = DateTime.UtcNow
        };

        await _repository.AddImportBatchAsync(batch, cancellationToken);

        var transactions = new List<PurchaseTransaction>();
        var totalRows = 0;

        string? line;
        while ((line = await reader.ReadLineAsync(cancellationToken)) is not null)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            totalRows++;
            var fields = SplitCsvLine(line);

            var customerId = GetField(fields, headerMap, "CustomerId");
            var transactionId = GetField(fields, headerMap, "TransactionId");
            var purchaseDateRaw = GetField(fields, headerMap, "PurchaseDate");
            var amountRaw = GetField(fields, headerMap, "Amount");

            if (!DateTime.TryParse(purchaseDateRaw, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var purchaseDate))
            {
                throw new CCSException(400, "Invalid CSV", $"Invalid PurchaseDate '{purchaseDateRaw}' on row {totalRows}.");
            }

            if (!decimal.TryParse(amountRaw, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount))
            {
                throw new CCSException(400, "Invalid CSV", $"Invalid Amount '{amountRaw}' on row {totalRows}.");
            }

            var lookup = await _customerLookupService.FindPersonAsync(customerId, cancellationToken);
            if (lookup is null)
            {
                throw new CCSException(
                    404,
                    "Customer not found",
                    $"CustomerId '{customerId}' does not exist in external directory.");
            }

            var customer = await _repository.GetCustomerByExternalRefAsync(customerId, cancellationToken);
            if (customer is null)
            {
                customer = new Customer
                {
                    CustomerId = Guid.NewGuid(),
                    ExternalCustomerRef = lookup.ExternalCustomerRef,
                    FirstName = lookup.FirstName,
                    LastName = lookup.LastName,
                    SourceSystem = SourceSystem.Csv,
                    CreatedAt = DateTime.UtcNow
                };
                await _repository.AddCustomerAsync(customer, cancellationToken);
            }

            transactions.Add(new PurchaseTransaction
            {
                PurchaseTransactionId = Guid.NewGuid(),
                ImportBatchId = batch.ImportBatchId,
                CampaignId = campaign.CampaignId,
                CustomerId = customer.CustomerId,
                TransactionId = transactionId,
                PurchaseDate = purchaseDate.ToUniversalTime(),
                Amount = amount,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _repository.AddPurchaseTransactionsAsync(transactions, cancellationToken);
        batch.Status = ImportBatchStatus.Processed;
        await _repository.SaveChangesAsync(cancellationToken);

        return new PurchaseImportResult(batch.ImportBatchId, totalRows, transactions.Count);
    }

    private static string ComputeSha256(byte[] bytes)
    {
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(bytes);
        var builder = new StringBuilder(hash.Length * 2);
        foreach (var b in hash)
        {
            builder.Append(b.ToString("x2"));
        }

        return builder.ToString();
    }

    private static List<string> SplitCsvLine(string line)
    {
        var result = new List<string>();
        if (string.IsNullOrEmpty(line))
        {
            return result;
        }

        var current = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var ch = line[i];

            if (ch == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                    continue;
                }

                inQuotes = !inQuotes;
                continue;
            }

            if (ch == ',' && !inQuotes)
            {
                result.Add(current.ToString().Trim());
                current.Clear();
                continue;
            }

            current.Append(ch);
        }

        result.Add(current.ToString().Trim());
        return result;
    }

    private static Dictionary<string, int> BuildHeaderMap(List<string> headers)
    {
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < headers.Count; i++)
        {
            var header = headers[i];
            if (!string.IsNullOrWhiteSpace(header) && !map.ContainsKey(header))
            {
                map[header] = i;
            }
        }

        return map;
    }

    private static void EnsureRequiredHeaders(Dictionary<string, int> headerMap)
    {
        var missing = RequiredHeaders.Where(h => !headerMap.ContainsKey(h)).ToList();
        if (missing.Count > 0)
        {
            throw new CCSException(400, "Invalid CSV", $"Missing required columns: {string.Join(", ", missing)}.");
        }
    }

    private static string GetField(List<string> fields, Dictionary<string, int> headerMap, string key)
    {
        if (!headerMap.TryGetValue(key, out var index) || index >= fields.Count)
        {
            return string.Empty;
        }

        return fields[index];
    }
}
