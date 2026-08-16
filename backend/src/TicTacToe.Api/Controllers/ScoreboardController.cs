using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Contracts.Scoreboard;
using TicTacToe.Api.Mappings;
using TicTacToe.Application.Services;

namespace TicTacToe.Api.Controllers;

[ApiController]
[Route("api/scoreboard")]
public class ScoreboardController : ControllerBase
{
    private readonly ScoreboardService _scoreboardService;

    public ScoreboardController(
        ScoreboardService scoreboardService)
    {
        _scoreboardService = scoreboardService;
    }

    [HttpGet]
    public ActionResult<ScoreboardResponse> GetScoreboard()
    {
        var scoreboard =
            _scoreboardService.GetScoreboard();

        return Ok(
            ScoreboardResponseMapper.ToResponse(scoreboard));
    }

    [HttpPost("reset")]
    public ActionResult<ScoreboardResponse> ResetScoreboard()
    {
        var scoreboard =
            _scoreboardService.ResetScoreboard();

        return Ok(
            ScoreboardResponseMapper.ToResponse(scoreboard));
    }
}