using System.ComponentModel.DataAnnotations;

namespace CustomerCampaignService.Api
{
    public sealed record LoginRequest([Required, MinLength(3), MaxLength(50)] string Username,[Required, MinLength(6), MaxLength(100)] string Password);
}
