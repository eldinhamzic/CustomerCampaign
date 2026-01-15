using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerCampaignService.Domain.Entities
{
    public sealed class Agent
    {
        public Guid AgentId { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        public ICollection<RewardEntry> RewardEntries { get; set; } = new List<RewardEntry>();
    }
}
