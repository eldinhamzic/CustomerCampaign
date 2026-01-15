using CustomerCampaignService.Domain.Entities;

namespace CustomerCampaignService.Application.Interfaces;

public interface IPurchaseImportRepository
{
    Task<Campaign?> GetCampaignByIdAsync(Guid campaignId, CancellationToken cancellationToken);
    Task<Campaign?> GetCampaignByCodeAsync(int campaignCode, CancellationToken cancellationToken);
    Task<Customer?> GetCustomerByExternalRefAsync(string externalRef, CancellationToken cancellationToken);
    Task AddCustomerAsync(Customer customer, CancellationToken cancellationToken);
    Task AddImportBatchAsync(PurchaseImport batch, CancellationToken cancellationToken);
    Task AddPurchaseTransactionsAsync(IEnumerable<PurchaseTransaction> transactions, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
