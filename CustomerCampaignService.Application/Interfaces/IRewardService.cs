using CustomerCampaignService.Application.Contracts.Rewards;

namespace CustomerCampaignService.Application.Interfaces;

public interface IRewardService
{
    Task<RewardEntryResponse> CreateRewardAsync(int campaignCode,CreateRewardRequest request,Guid agentId,CancellationToken cancellationToken);

    Task CancelRewardAsync(int rewardCode,Guid agentId,CancellationToken cancellationToken);

    Task<RewardEntryResponse> CorrectRewardAsync(int rewardCode,CorrectRewardRequest request, Guid agentId, CancellationToken cancellationToken);
}
