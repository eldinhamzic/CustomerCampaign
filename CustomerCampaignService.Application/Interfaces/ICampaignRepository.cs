using CustomerCampaignService.Domain.Entities;

namespace CustomerCampaignService.Application.Interfaces;

public interface ICampaignRepository
{
    Task<IReadOnlyList<Campaign>> GetCampaignsAsyncFromDb(CancellationToken cancellationToken);
}
