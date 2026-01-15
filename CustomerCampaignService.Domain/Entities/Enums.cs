using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerCampaignService.Domain.Entities
{
    public enum SourceSystem
    {
        Soap = 1,
        Csv = 2,
        Manual = 3
    }

    public enum RewardEntryStatus
    {
        Valid = 1,
        Corrected = 2,
        Cancelled = 3
    }

    public enum ImportBatchStatus
    {
        Received = 1,
        Processed = 2,
        Failed = 3
    }

}
