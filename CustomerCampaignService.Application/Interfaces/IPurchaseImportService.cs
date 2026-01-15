using CustomerCampaignService.Application.Contracts.Purchases;

namespace CustomerCampaignService.Application.Interfaces;

public interface IPurchaseImportService
{
    Task<PurchaseImportResult> ImportFileAsync(int campaignCode,string fileName,Stream csvStream,CancellationToken cancellationToken);
}
