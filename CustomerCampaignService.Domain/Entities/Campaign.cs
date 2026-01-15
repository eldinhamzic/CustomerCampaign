using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerCampaignService.Domain.Entities
{
    public sealed class Campaign
    {
        public Guid CampaignId { get; set; }
        public int CampaignCode { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<RewardEntry> RewardEntries { get; set; } = new List<RewardEntry>();
        public ICollection<PurchaseImport> PurchaseImportBatches { get; set; } = new List<PurchaseImport>();
    }

}
