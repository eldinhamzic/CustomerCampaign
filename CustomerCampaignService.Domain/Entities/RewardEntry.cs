using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerCampaignService.Domain.Entities
{
    public sealed class RewardEntry
    {
        public Guid RewardEntryId { get; set; }
        public int RewardCode { get; set; }
        public Guid CampaignId { get; set; }
        public Guid AgentId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? CorrectionOfRewardEntryId { get; set; }
        public DateOnly RewardDate { get; set; }
        public RewardEntryStatus Status { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        public Campaign? Campaign { get; set; }
        public Agent? Agent { get; set; }
        public Customer? Customer { get; set; }
        public RewardEntry? CorrectionOfRewardEntry { get; set; }
        public ICollection<RewardEntry> Corrections { get; set; } = new List<RewardEntry>();
    }
}
