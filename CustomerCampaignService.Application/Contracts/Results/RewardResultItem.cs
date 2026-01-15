namespace CustomerCampaignService.Application.Contracts.Results;

public sealed record RewardResultItem(Guid RewardEntryId,int RewardCode,Guid CampaignId,int CampaignCode,Guid AgentId,Guid CustomerId, DateOnly RewardDate,bool Purchased);
