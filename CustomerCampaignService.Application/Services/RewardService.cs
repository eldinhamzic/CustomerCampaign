using CustomerCampaignService.Application.Contracts.Rewards;
using CustomerCampaignService.Application.Errors;
using CustomerCampaignService.Application.Interfaces;
using CustomerCampaignService.Domain.Entities;

namespace CustomerCampaignService.Application.Services;

public sealed class RewardService : IRewardService
{
    private readonly IPurchaseImportRepository _purchaseService;
    private readonly IRewardRepository _rewardRepository;
    private readonly ICustomerLookupService _customerLookupService;

    public RewardService(IRewardRepository rewardRepository, ICustomerLookupService customerLookupService, IPurchaseImportRepository purchaseImportRepository)
    {
        _rewardRepository = rewardRepository;
        _customerLookupService = customerLookupService;
        _purchaseService = purchaseImportRepository;
    }

    public async Task<RewardEntryResponse> CreateRewardAsync(int campaignCode,CreateRewardRequest request,Guid agentId,CancellationToken cancellationToken)
    {
        var campaign = await _purchaseService.GetCampaignByCodeAsync(campaignCode, cancellationToken);
        if (campaign is null)
        {
            throw new CCSException(404, "Campaign not found", $"CampaignCode '{campaignCode}' does not exist.");
        }

        if (!campaign.IsActive || request.RewardDate < campaign.StartDate || request.RewardDate > campaign.EndDate)
        {
            throw new CCSException(
                400,
                "Campaign not active",
                "Reward date is outside the active campaign window.");
        }

        var dailyCount = await _rewardRepository.CountDailyRewardsAsync(agentId,request.RewardDate,cancellationToken);
        if (dailyCount >= 5)
        {
            throw new CCSException(
                400,
                "Daily reward limit exceeded",
                "Agent already rewarded 5 customers today.");
        }

        var lookup = await _customerLookupService.FindPersonAsync(request.CustomerId, cancellationToken);
        if (lookup is null)
        {
            throw new CCSException(
                404,
                "Customer not found",
                $"CustomerId '{request.CustomerId}' does not exist in external directory.");
        }

        var customer = await _rewardRepository.GetCustomerByExternalRefAsync(
            request.CustomerId,
            cancellationToken);
        if (customer is null)
        {
            customer = new Customer
            {
                CustomerId = Guid.NewGuid(),
                ExternalCustomerRef = lookup.ExternalCustomerRef,
                FirstName = lookup.FirstName,
                LastName = lookup.LastName,
                SourceSystem = SourceSystem.Soap,
                CreatedAt = DateTime.UtcNow
            };
            await _rewardRepository.AddCustomerAsync(customer, cancellationToken);
        }

        var hasValidReward = await _rewardRepository.HasValidRewardForCustomerAsync(
            campaign.CampaignId,
            customer.CustomerId,
            cancellationToken);
        if (hasValidReward)
        {
            throw new CCSException(
                400,
                "Duplicate reward",
                "Customer already has a valid reward in this campaign.");
        }

        var entry = new RewardEntry
        {
            RewardEntryId = Guid.NewGuid(),
            CampaignId = campaign.CampaignId,
            AgentId = agentId,
            CustomerId = customer.CustomerId,
            CorrectionOfRewardEntryId = null,
            RewardDate = request.RewardDate,
            Status = RewardEntryStatus.Valid,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _rewardRepository.AddRewardEntryAsync(entry, cancellationToken);
        await _rewardRepository.SaveChangesAsync(cancellationToken);
        entry = await _rewardRepository.GetRewardEntryAsync(entry.RewardEntryId, cancellationToken)
            ?? entry;

        return new RewardEntryResponse(
            entry.RewardEntryId,
            entry.RewardCode,
            entry.CampaignId,
            campaign.CampaignCode,
            entry.AgentId,
            entry.CustomerId,
            entry.RewardDate,
            entry.Status.ToString(),
            entry.Notes,
            entry.CreatedAt);
    }

    public async Task CancelRewardAsync(int rewardCode,Guid agentId,CancellationToken cancellationToken)
    {
        var entry = await _rewardRepository.GetRewardEntryByCodeAsync(rewardCode, cancellationToken);
        if (entry is null)
        {
            throw new CCSException(404, "Reward not found", $"RewardCode '{rewardCode}' does not exist.");
        }

        if (entry.AgentId != agentId)
        {
            throw new CCSException(403, "Forbidden", "You can only cancel your own reward entries.");
        }

        if (entry.Status == RewardEntryStatus.Cancelled)
        {
            throw new CCSException(400, "Already cancelled", "Reward is already cancelled.");
        }

        if (entry.Status == RewardEntryStatus.Corrected)
        {
            throw new CCSException(400, "Already corrected", "Reward was already corrected.");
        }

        entry.Status = RewardEntryStatus.Cancelled;
        await _rewardRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<RewardEntryResponse> CorrectRewardAsync(int rewardCode,CorrectRewardRequest request,Guid agentId,CancellationToken cancellationToken)
    {
        var original = await _rewardRepository.GetRewardEntryByCodeAsync(rewardCode, cancellationToken);
        if (original is null)
        {
            throw new CCSException(404, "Reward not found", $"RewardCode '{rewardCode}' does not exist.");
        }

        if (original.Status == RewardEntryStatus.Cancelled)
        {
            throw new CCSException(400, "Cannot correct", "Reward is cancelled.");
        }

        if (original.AgentId != agentId)
        {
            throw new CCSException(403, "Forbidden", "You can only correct your own reward entries.");
        }

        if (original.Status == RewardEntryStatus.Corrected)
        {
            throw new CCSException(400, "Cannot correct", "Reward was already corrected.");
        }

        var campaign = await _purchaseService.GetCampaignByIdAsync(original.CampaignId, cancellationToken);
        if (campaign is null)
        {
            throw new CCSException(404, "Campaign not found", $"CampaignId '{original.CampaignId}' does not exist.");
        }

        if (!campaign.IsActive || request.RewardDate < campaign.StartDate || request.RewardDate > campaign.EndDate)
        {
            throw new CCSException(
                400,
                "Campaign not active",
                "Reward date is outside the active campaign window.");
        }

        var dailyCount = await _rewardRepository.CountDailyRewardsAsync(agentId,request.RewardDate,cancellationToken);
        if (dailyCount >= 5)
        {
            throw new CCSException(
                400,
                "Daily reward limit exceeded",
                "Agent already rewarded 5 customers today.");
        }

        var lookup = await _customerLookupService.FindPersonAsync(request.CustomerId,cancellationToken);
        if (lookup is null)
        {
            throw new CCSException(
                404,
                "Customer not found",
                $"CustomerId '{request.CustomerId}' does not exist in external directory.");
        }

        var customer = await _rewardRepository.GetCustomerByExternalRefAsync(request.CustomerId,cancellationToken);
        if (customer is null)
        {
            customer = new Customer
            {
                CustomerId = Guid.NewGuid(),
                ExternalCustomerRef = lookup.ExternalCustomerRef,
                FirstName = lookup.FirstName,
                LastName = lookup.LastName,
                SourceSystem = SourceSystem.Soap,
                CreatedAt = DateTime.UtcNow
            };
            await _rewardRepository.AddCustomerAsync(customer, cancellationToken);
        }

        if (customer.CustomerId != original.CustomerId)
        {
            var hasValidReward = await _rewardRepository.HasValidRewardForCustomerAsync(campaign.CampaignId,customer.CustomerId,cancellationToken);
            if (hasValidReward)
            {
                throw new CCSException(
                    400,
                    "Duplicate reward",
                    "Customer already has a valid reward in this campaign.");
            }
        }

        original.Status = RewardEntryStatus.Corrected;

        var correctedEntry = new RewardEntry
        {
            RewardEntryId = Guid.NewGuid(),
            CampaignId = original.CampaignId,
            AgentId = agentId,
            CustomerId = customer.CustomerId,
            CorrectionOfRewardEntryId = original.RewardEntryId,
            RewardDate = request.RewardDate,
            Status = RewardEntryStatus.Valid,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _rewardRepository.AddRewardEntryAsync(correctedEntry, cancellationToken);
        await _rewardRepository.SaveChangesAsync(cancellationToken);
        correctedEntry = await _rewardRepository.GetRewardEntryAsync(correctedEntry.RewardEntryId, cancellationToken) ?? correctedEntry;

        return new RewardEntryResponse(
            correctedEntry.RewardEntryId,
            correctedEntry.RewardCode,
            correctedEntry.CampaignId,
            campaign.CampaignCode,
            correctedEntry.AgentId,
            correctedEntry.CustomerId,
            correctedEntry.RewardDate,
            correctedEntry.Status.ToString(),
            correctedEntry.Notes,
            correctedEntry.CreatedAt);
    }
}
