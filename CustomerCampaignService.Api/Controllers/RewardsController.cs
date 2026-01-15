using CustomerCampaignService.Application.Contracts.Rewards;
using CustomerCampaignService.Application.Errors;
using CustomerCampaignService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerCampaignService.Api.Controllers;

[ApiController]
[Route("api/campaigns")]
public sealed class RewardsController : ControllerBase
{
    private readonly IRewardService _rewardService;
    private readonly IRewardRepository _rewardRepository;

    public RewardsController(IRewardService rewardService, IRewardRepository rewardRepository)
    {
        _rewardService = rewardService;
        _rewardRepository = rewardRepository;
    }

    [Authorize(Roles = "Agent")]
    [HttpPost("{campaignCode}/rewards)")]
    [ProducesResponseType(typeof(RewardEntryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RewardEntryResponse>> CreateReward(int campaignCode,[FromBody] CreateRewardRequest request, CancellationToken cancellationToken)
    {
        var agentUsername = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(agentUsername))
        {
            return Unauthorized();
        }

        var agent = await _rewardRepository.GetAgentByUsernameAsync(agentUsername, cancellationToken);
        if (agent is null)
        {
            throw new CCSException(404, "Agent not found", $"Agent '{agentUsername}' does not exist.");
        }

        var response = await _rewardService.CreateRewardAsync(
            campaignCode,
            request,
            agent.AgentId,
            cancellationToken);

        return Ok(response);
    }


    [Authorize(Roles = "Agent")]
    [HttpPost("{code}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelReward(int code, CancellationToken cancellationToken)
    {
        var agentUsername = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(agentUsername))
        {
            return Unauthorized();
        }

        var agent = await _rewardRepository.GetAgentByUsernameAsync(agentUsername, cancellationToken);
        if (agent is null)
        {
            throw new CCSException(404, "Agent not found", $"Agent '{agentUsername}' does not exist.");
        }

        await _rewardService.CancelRewardAsync(code, agent.AgentId, cancellationToken);
        return Ok(new { ok = true });
    }

    [Authorize(Roles = "Agent")]
    [HttpPost("{code}/correct")]
    [ProducesResponseType(typeof(RewardEntryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RewardEntryResponse>> CorrectRewards(int code,[FromBody] CorrectRewardRequest request,CancellationToken cancellationToken)
    {
        var agentUsername = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(agentUsername))
        {
            return Unauthorized();
        }

        var agent = await _rewardRepository.GetAgentByUsernameAsync(agentUsername, cancellationToken);
        if (agent is null)
        {
            throw new CCSException(404, "Agent not found", $"Agent '{agentUsername}' does not exist.");
        }

        var response = await _rewardService.CorrectRewardAsync(
            code,
            request,
            agent.AgentId,
            cancellationToken);

        return Ok(response);
    }
}
