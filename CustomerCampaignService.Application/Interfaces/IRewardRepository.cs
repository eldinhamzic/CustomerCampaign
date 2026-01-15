using CustomerCampaignService.Domain.Entities;

namespace CustomerCampaignService.Application.Interfaces;

public interface IRewardRepository
{
 
    Task<Agent?> GetAgentByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<Customer?> GetCustomerByExternalRefAsync(string externalRef, CancellationToken cancellationToken);
    Task AddCustomerAsync(Customer customer, CancellationToken cancellationToken);
    Task<bool> HasValidRewardForCustomerAsync( Guid campaignId, Guid customerId, CancellationToken cancellationToken);
    Task<RewardEntry?> GetRewardEntryAsync( Guid rewardEntryId, CancellationToken cancellationToken);
    Task<RewardEntry?> GetRewardEntryByCodeAsync(int rewardCode, CancellationToken cancellationToken);
    Task<int> CountDailyRewardsAsync(Guid agentId, DateOnly rewardDate, CancellationToken cancellationToken);
    Task AddRewardEntryAsync(RewardEntry entry, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
