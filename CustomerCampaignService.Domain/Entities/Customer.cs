using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerCampaignService.Domain.Entities
{
    public sealed class Customer
    {
        public Guid CustomerId { get; set; }
        public string ExternalCustomerRef { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public SourceSystem SourceSystem { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<RewardEntry> RewardEntries { get; set; } = new List<RewardEntry>();
    }
}
