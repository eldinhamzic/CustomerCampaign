using CustomerCampaignService.Application.Contracts.Results;
using CustomerCampaignService.Application.Errors;
using CustomerCampaignService.Application.Interfaces;

namespace CustomerCampaignService.Application.Services;

public sealed class ResultsService : IResultsService
{
    private readonly IPurchaseImportRepository _purchaseImport;
    private readonly IResultsRepository _resultsRepo;

    public ResultsService(IPurchaseImportRepository purchaseImport,IResultsRepository resultsRepo)
    {
        _purchaseImport = purchaseImport;
        _resultsRepo = resultsRepo;
    }

    public async Task<RewardResultsResponse> GetCampaignResultsAsync(int campaignCode,DateOnly? from,DateOnly? to,CancellationToken cancellationToken)
    {
        var campaign = await _purchaseImport.GetCampaignByCodeAsync(campaignCode, cancellationToken);
        if (campaign is null)
        {
            throw new CCSException(404, "Campaign not found", $"CampaignCode '{campaignCode}' does not exist.");
        }

        if (from.HasValue && to.HasValue && from > to)
        {
            throw new CCSException(400, "Invalid range", "'from' cannot be after 'to'.");
        }

        var rewards = await _resultsRepo.GetValidRewardsAsync(campaign.CampaignId, from, to, cancellationToken);
        var purchases = await _resultsRepo.GetPurchasedCustomersAsync(campaign.CampaignId, from, to, cancellationToken);

        var purchasesByCustomer = purchases.GroupBy(x => x.CustomerId).ToDictionary(g => g.Key, g => g.Select(x => x.PurchaseDate).ToList());

        var items = rewards.Select(reward =>
        {
            var purchased = false;
            if (purchasesByCustomer.TryGetValue(reward.CustomerId, out var customerPurchases))
            {
                purchased = customerPurchases.Any(p =>
                    DateOnly.FromDateTime(p) >= reward.RewardDate);
            }

            return new RewardResultItem(
                reward.RewardEntryId,
                reward.RewardCode,
                reward.CampaignId,
                campaign.CampaignCode,
                reward.AgentId,
                reward.CustomerId,
                reward.RewardDate,
                purchased);
        }).ToList();

        var total = items.Count;
        var purchased = items.Count(x => x.Purchased);
        var notPurchased = total - purchased;

        return new RewardResultsResponse(items, total, purchased, notPurchased);
    }
}
