using CustomerCampaignService.Application.Interfaces;
using CustomerCampaignService.Domain.Entities;
using CustomerCampaignService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerCampaignService.Infrastructure.Repositories;

public sealed class RewardRepository : IRewardRepository
{
    private readonly AppDbContext _dbContext;

    public RewardRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

  
    public Task<Agent?> GetAgentByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return _dbContext.Agents.FirstOrDefaultAsync(x => x.Username == username, cancellationToken);
    }

    public Task<Customer?> GetCustomerByExternalRefAsync(string externalRef, CancellationToken cancellationToken)
    {
        return _dbContext.Customers.FirstOrDefaultAsync(x => x.ExternalCustomerRef == externalRef, cancellationToken);
    }

    public Task AddCustomerAsync(Customer customer, CancellationToken cancellationToken)
    {
        return _dbContext.Customers.AddAsync(customer, cancellationToken).AsTask();
    }

    public Task<bool> HasValidRewardForCustomerAsync(Guid campaignId,Guid customerId,CancellationToken cancellationToken)
    {
        return _dbContext.RewardEntries.AsNoTracking().AnyAsync(x => x.CampaignId == campaignId && x.CustomerId == customerId && x.Status == RewardEntryStatus.Valid,cancellationToken);
    }

    public Task<RewardEntry?> GetRewardEntryAsync(Guid rewardEntryId, CancellationToken cancellationToken)
    {
        return _dbContext.RewardEntries.FirstOrDefaultAsync(x => x.RewardEntryId == rewardEntryId, cancellationToken);
    }

    public Task<RewardEntry?> GetRewardEntryByCodeAsync(int rewardCode, CancellationToken cancellationToken)
    {
        return _dbContext.RewardEntries.FirstOrDefaultAsync(x => x.RewardCode == rewardCode, cancellationToken);
    }

    public Task<int> CountDailyRewardsAsync(Guid agentId, DateOnly rewardDate, CancellationToken cancellationToken)
    {
        return _dbContext.RewardEntries.AsNoTracking().CountAsync(
                x => x.AgentId == agentId
                     && x.RewardDate == rewardDate
                     && x.Status == RewardEntryStatus.Valid,cancellationToken);
    }

    public Task AddRewardEntryAsync(RewardEntry entry, CancellationToken cancellationToken)
    {
        return _dbContext.RewardEntries.AddAsync(entry, cancellationToken).AsTask();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
