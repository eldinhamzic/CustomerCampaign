using CustomerCampaignService.Application.Contracts.Results;

namespace CustomerCampaignService.Application.Interfaces;

public interface IResultsService
{
    Task<RewardResultsResponse> GetCampaignResultsAsync( int campaignCode, DateOnly? from,  DateOnly? to, CancellationToken cancellationToken);
}
