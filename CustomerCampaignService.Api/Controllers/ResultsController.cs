using CustomerCampaignService.Application.Contracts.Results;
using CustomerCampaignService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerCampaignService.Api.Controllers;

[ApiController]
[Route("api/campaigns")]
public sealed class ResultsController : ControllerBase
{
    private readonly IResultsService _resultsService;

    public ResultsController(IResultsService resultsService)
    {
        _resultsService = resultsService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{campaignCode}/results")]
    [ProducesResponseType(typeof(RewardResultsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RewardResultsResponse>> GetResults(int campaignCode,[FromQuery] DateOnly? from,[FromQuery] DateOnly? to,CancellationToken cancellationToken)
    {
        var response = await _resultsService.GetCampaignResultsAsync(
            campaignCode,
            from,
            to,
            cancellationToken);

        return Ok(response);
    }
}
