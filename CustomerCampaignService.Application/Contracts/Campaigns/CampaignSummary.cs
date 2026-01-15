namespace CustomerCampaignService.Application.Contracts.Campaigns;

public sealed record CampaignSummary(int CampaignCode,string Name,DateOnly StartDate,DateOnly EndDate,bool IsActive);
