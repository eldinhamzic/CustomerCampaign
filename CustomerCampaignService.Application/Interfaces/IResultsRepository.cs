using CustomerCampaignService.Domain.Entities;

namespace CustomerCampaignService.Application.Interfaces;

public interface IResultsRepository
{

    Task<List<RewardEntry>> GetValidRewardsAsync(Guid campaignId, DateOnly? from, DateOnly? to, CancellationToken cancellationToken);
    Task<List<(Guid CustomerId, DateTime PurchaseDate)>> GetPurchasedCustomersAsync( Guid campaignId, DateOnly? from, DateOnly? to, CancellationToken cancellationToken);
}
