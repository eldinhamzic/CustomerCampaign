namespace CustomerCampaignService.Application.Contracts.Results;

public sealed record RewardResultsResponse(IReadOnlyList<RewardResultItem> Items, int TotalRewards, int PurchasedRewards, int NotPurchasedRewards);
