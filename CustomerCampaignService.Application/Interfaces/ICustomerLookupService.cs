using CustomerCampaignService.Application.Contracts.Customers;

namespace CustomerCampaignService.Application.Interfaces;

public interface ICustomerLookupService
{
    Task<CustomerLookupResult?> FindPersonAsync(string externalCustomerRef, CancellationToken cancellationToken);
}
