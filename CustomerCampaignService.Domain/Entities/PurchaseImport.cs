using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerCampaignService.Domain.Entities
{
    public sealed class PurchaseImport
    {
        public Guid ImportBatchId { get; set; }
        public Guid CampaignId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileHash { get; set; } = string.Empty;
        public ImportBatchStatus Status { get; set; }
        public DateTime ImportedAt { get; set; }
        public string? ErrorMessage { get; set; }

        public Campaign? Campaign { get; set; }
    }
}
