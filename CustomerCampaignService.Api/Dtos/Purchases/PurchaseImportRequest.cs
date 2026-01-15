namespace CustomerCampaignService.Api.Dtos.Purchases;

public sealed class PurchaseImportRequest
{
    public IFormFile File { get; set; } = default!;
}
