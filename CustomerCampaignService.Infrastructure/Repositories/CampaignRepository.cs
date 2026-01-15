using CustomerCampaignService.Application.Interfaces;
using CustomerCampaignService.Domain.Entities;
using CustomerCampaignService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerCampaignService.Infrastructure.Repositories;

public sealed class CampaignRepository : ICampaignRepository
{
    private readonly AppDbContext _dbContext;

    public CampaignRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Campaign>> GetCampaignsAsyncFromDb(CancellationToken cancellationToken)
    {
        var campaigns = await _dbContext.Campaigns.AsNoTracking().ToListAsync(cancellationToken);

        return campaigns;
    }
}
