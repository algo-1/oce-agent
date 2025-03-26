using Microsoft.AspNetCore.Mvc;
using OncallAgent.Models;
using OncallAgent.Services;

namespace OncallAgent.Controllers;

[ApiController]
[Route("api/tsg")]
public class TsgController : ControllerBase
{
    private readonly TsgService _tsgService;

    public TsgController(TsgService tsgService)
    {
        _tsgService = tsgService;
    }

    [HttpPost]
    public async Task<IActionResult> AddTsg([FromBody] TsgModel tsgModel)
    {
        if (tsgModel == null || string.IsNullOrEmpty(tsgModel.Title) || tsgModel.Steps == null || !tsgModel.Steps.Any())
        {
            return BadRequest("Invalid TSG data. Please provide a title and at least one step.");
        }

        // Save the TSG using TsgService
        await _tsgService.IndexSingleTsgAsync(tsgModel);

        return Ok(new { message = "TSG added successfully!" });
    }
}
