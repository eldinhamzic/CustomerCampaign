using CustomerCampaignService.Api.Dtos.Purchases;
using CustomerCampaignService.Application.Contracts.Purchases;
using CustomerCampaignService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerCampaignService.Api.Controllers;

[ApiController]
[Route("api/campaigns")]
public sealed class PurchaseImportController : ControllerBase
{
    private readonly IPurchaseImportService _importService;

    public PurchaseImportController(IPurchaseImportService importService)
    {
        _importService = importService;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{campaignCode}/purchases/import")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(PurchaseImportResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PurchaseImportResult>> ImportFile(int campaignCode,[FromForm] PurchaseImportRequest request,CancellationToken cancellationToken)
    {
        if (request.File is null || request.File.Length == 0)
        {
            return BadRequest("CSV file is required.");
        }

        await using var stream = request.File.OpenReadStream();
        var result = await _importService.ImportFileAsync(campaignCode,request.File.FileName,stream,cancellationToken);

        return Ok(result);
    }
}
