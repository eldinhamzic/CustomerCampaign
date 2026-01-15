using System.ComponentModel.DataAnnotations;

namespace CustomerCampaignService.Application.Contracts.Rewards;

public sealed record CreateRewardRequest([Required] string CustomerId,[Required] DateOnly RewardDate,string? Notes);
