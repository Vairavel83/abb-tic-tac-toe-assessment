using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Contracts.Games;
using TicTacToe.Api.Mappings;
using TicTacToe.Application.Services;

namespace TicTacToe.Api.Controllers;

[ApiController]
[Route("api/games")]
public class GamesController : ControllerBase
{
    private readonly GameService _gameService;

    public GamesController(GameService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost]
    public ActionResult<GameStateResponse> CreateGame(
        CreateGameRequest request)
    {
        var game = _gameService.CreateGame(request.Mode);

        var response =
            GameResponseMapper.ToResponse(game);

        return CreatedAtAction(
            nameof(GetGame),
            new { id = game.Id },
            response);
    }

    [HttpGet("{id:guid}")]
    public ActionResult<GameStateResponse> GetGame(Guid id)
    {
        try
        {
            var game = _gameService.GetGame(id);

            return Ok(
                GameResponseMapper.ToResponse(game));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Game not found",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
    }
    [HttpPost("{id:guid}/moves")]
public ActionResult<GameStateResponse> MakeMove(
    Guid id,
    MoveRequest request)
{
    try
    {
        var game = _gameService.MakeMove(
            id,
            request.Player,
            request.Position);

        return Ok(
            GameResponseMapper.ToResponse(game));
    }
    catch (KeyNotFoundException ex)
    {
        return NotFound(new ProblemDetails
        {
            Title = "Game not found",
            Detail = ex.Message,
            Status = StatusCodes.Status404NotFound
        });
    }
    catch (ArgumentOutOfRangeException ex)
    {
        return BadRequest(new ProblemDetails
        {
            Title = "Invalid board position",
            Detail = ex.Message,
            Status = StatusCodes.Status400BadRequest
        });
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(new ProblemDetails
        {
            Title = "Invalid move",
            Detail = ex.Message,
            Status = StatusCodes.Status400BadRequest
        });
    }
}

[HttpPost("{id:guid}/undo")]
public ActionResult<GameStateResponse> Undo(Guid id)
{
    try
    {
        var game = _gameService.Undo(id);

        return Ok(
            GameResponseMapper.ToResponse(game));
    }
    catch (KeyNotFoundException ex)
    {
        return NotFound(new ProblemDetails
        {
            Title = "Game not found",
            Detail = ex.Message,
            Status = StatusCodes.Status404NotFound
        });
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(new ProblemDetails
        {
            Title = "Unable to undo",
            Detail = ex.Message,
            Status = StatusCodes.Status400BadRequest
        });
    }
}
}