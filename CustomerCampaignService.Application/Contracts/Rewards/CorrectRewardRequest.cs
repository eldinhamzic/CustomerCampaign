using System.ComponentModel.DataAnnotations;

namespace CustomerCampaignService.Application.Contracts.Rewards;

public sealed record CorrectRewardRequest([Required] string CustomerId,[Required] DateOnly RewardDate,string? Notes);
