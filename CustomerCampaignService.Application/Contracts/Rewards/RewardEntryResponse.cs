namespace CustomerCampaignService.Application.Contracts.Rewards;

public sealed record RewardEntryResponse(Guid RewardEntryId, int RewardCode,Guid CampaignId,int CampaignCode,Guid AgentId, Guid CustomerId,DateOnly RewardDate,string Status,string? Notes, DateTime CreatedAt);
