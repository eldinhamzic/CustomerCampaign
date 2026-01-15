namespace CustomerCampaignService.Application.Contracts.Purchases;

public sealed record PurchaseImportResult(Guid ImportBatchId,int TotalRows,int ImportedRows);
