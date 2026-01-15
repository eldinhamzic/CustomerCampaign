using CustomerCampaignService.Application.Contracts.Campaigns;
using CustomerCampaignService.Application.Interfaces;

namespace CustomerCampaignService.Application.Services;

public sealed class CampaignService : ICampaignService
{
    private readonly ICampaignRepository _repository;

    public CampaignService(ICampaignRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<CampaignSummary>> GetCampaignsAsync(CancellationToken cancellationToken)
    {
        var campaigns = await _repository.GetCampaignsAsyncFromDb(cancellationToken);
        return campaigns.OrderBy(x => x.CampaignCode).Select(x => 
            new CampaignSummary(
                x.CampaignCode,
                x.Name,
                x.StartDate,
                x.EndDate,
                x.IsActive)).ToList();
    }
}
