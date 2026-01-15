using CustomerCampaignService.Application.Contracts.Campaigns;
using CustomerCampaignService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerCampaignService.Api.Controllers;

[ApiController]
[Route("api/campaigns")]
public sealed class CampaignsController : ControllerBase
{
    private readonly ICampaignService _campaignQueryService;

    public CampaignsController(ICampaignService campaignQueryService)
    {
        _campaignQueryService = campaignQueryService;
    }

    [Authorize(Roles = "Admin,Agent")]
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CampaignSummary>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<CampaignSummary>>> GetCampings(
        CancellationToken cancellationToken)
    {
        var campaigns = await _campaignQueryService.GetCampaignsAsync(cancellationToken);
        return Ok(campaigns);
    }
}
