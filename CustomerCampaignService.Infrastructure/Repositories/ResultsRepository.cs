using CustomerCampaignService.Application.Interfaces;
using CustomerCampaignService.Domain.Entities;
using CustomerCampaignService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerCampaignService.Infrastructure.Repositories;

public sealed class ResultsRepository : IResultsRepository
{
    private readonly AppDbContext _dbContext;

    public ResultsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public Task<List<RewardEntry>> GetValidRewardsAsync(Guid campaignId,DateOnly? dateFrom,DateOnly? dateTo,CancellationToken cancellationToken)
    {
        var query = _dbContext.RewardEntries.AsNoTracking().Where(x => x.CampaignId == campaignId && x.Status == RewardEntryStatus.Valid);

        if (dateFrom.HasValue)
        {
            query = query.Where(x => x.RewardDate >=    dateFrom.Value);
        }

        if (dateTo.HasValue)
        {
            query = query.Where(x => x.RewardDate <= dateTo.Value);
        }

        return query.ToListAsync(cancellationToken);
    }

    public Task<List<(Guid CustomerId, DateTime PurchaseDate)>> GetPurchasedCustomersAsync(Guid campaignId,DateOnly? from,DateOnly? to,CancellationToken cancellationToken)
    {
        var query = _dbContext.PurchaseTransactions.AsNoTracking().Where(x => x.CampaignId == campaignId);

        if (from.HasValue)
        {
            var start = from.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(x => x.PurchaseDate >= start);
        }

        if (to.HasValue)
        {
            var end = to.Value.ToDateTime(TimeOnly.MaxValue);
            query = query.Where(x => x.PurchaseDate <= end);
        }

        return query.Select(x => new ValueTuple<Guid, DateTime>(x.CustomerId, x.PurchaseDate)).ToListAsync(cancellationToken);
    }
}
