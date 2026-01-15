using CustomerCampaignService.Domain.Entities;
using CustomerCampaignService.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using System;

namespace CustomerCampaignService.Seeds;

public static class BusinessSeed
{
    public static async Task BussinesSeedAsync(AppDbContext dbContext)
    {
        if (!await dbContext.Agents.AnyAsync())
        {
            dbContext.Agents.Add(new Agent
            {
                AgentId = Guid.NewGuid(),
                Username = "agent1",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            });
        }

        if (!await dbContext.Campaigns.AnyAsync())
        {
            var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
            dbContext.Campaigns.Add(new Campaign
            {
                CampaignId = Guid.NewGuid(),
                CampaignCode = 100,
                Name = "Loyalty Week",
                StartDate = startDate,
                EndDate = startDate.AddDays(7),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }
        else
        {
            var existing = await dbContext.Campaigns.FirstOrDefaultAsync();
            if (existing is not null && existing.CampaignCode == 0)
            {
                existing.CampaignCode = 100;
            }
        }

        if (!await dbContext.Customers.AnyAsync())
        {
            dbContext.Customers.Add(new Customer
            {
                CustomerId = Guid.NewGuid(),
                ExternalCustomerRef = "101",
                FirstName = "Peter",
                LastName = "Donaldson",
                SourceSystem = SourceSystem.Csv,
                CreatedAt = DateTime.UtcNow
            });

            dbContext.Customers.Add(new Customer
            {
                CustomerId = Guid.NewGuid(),
                ExternalCustomerRef = "100",
                FirstName = "Rob",
                LastName = "Wilson",
                SourceSystem = SourceSystem.Csv,
                CreatedAt = DateTime.UtcNow
            });
        }

        await dbContext.SaveChangesAsync();
    }
}
