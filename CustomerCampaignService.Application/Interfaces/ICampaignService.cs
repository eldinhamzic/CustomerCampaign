using CustomerCampaignService.Application.Contracts.Campaigns;

namespace CustomerCampaignService.Application.Interfaces;

public interface ICampaignService
{
    Task<IReadOnlyList<CampaignSummary>> GetCampaignsAsync(CancellationToken cancellationToken);
}
