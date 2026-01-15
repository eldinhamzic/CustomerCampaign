using CustomerCampaignService.Application.Interfaces;
using CustomerCampaignService.Domain.Entities;
using CustomerCampaignService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerCampaignService.Infrastructure.Repositories;

public sealed class PurchaseImportRepository : IPurchaseImportRepository
{
    private readonly AppDbContext _dbContext;

    public PurchaseImportRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Campaign?> GetCampaignByIdAsync(Guid campaignId, CancellationToken cancellationToken)
    {
        return  _dbContext.Campaigns.AsNoTracking().FirstOrDefaultAsync(x => x.CampaignId == campaignId, cancellationToken);
    }

    public Task<Campaign?> GetCampaignByCodeAsync(int campaignCode, CancellationToken cancellationToken)
    {
        return _dbContext.Campaigns.AsNoTracking().FirstOrDefaultAsync(x => x.CampaignCode == campaignCode, cancellationToken);
    }

    public Task<Customer?> GetCustomerByExternalRefAsync(string externalRef, CancellationToken cancellationToken)
    {
        return _dbContext.Customers.FirstOrDefaultAsync(x => x.ExternalCustomerRef == externalRef, cancellationToken);
    }

    public Task AddCustomerAsync(Customer customer, CancellationToken cancellationToken)
    {
        return _dbContext.Customers.AddAsync(customer, cancellationToken).AsTask();
    }

    public Task AddImportBatchAsync(PurchaseImport batch, CancellationToken cancellationToken)
    {
        return _dbContext.PurchaseImports.AddAsync(batch, cancellationToken).AsTask();
    }

    public Task AddPurchaseTransactionsAsync( IEnumerable<PurchaseTransaction> transactions,CancellationToken cancellationToken)
    {
        return _dbContext.PurchaseTransactions.AddRangeAsync(transactions, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
