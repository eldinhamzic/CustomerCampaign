using System;

namespace CustomerCampaignService.Domain.Entities
{
    public sealed class PurchaseTransaction
    {
        public Guid PurchaseTransactionId { get; set; }
        public Guid ImportBatchId { get; set; }
        public Guid CampaignId { get; set; }
        public Guid CustomerId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }

        public PurchaseImport? ImportBatch { get; set; }
        public Campaign? Campaign { get; set; }
        public Customer? Customer { get; set; }
    }
}
