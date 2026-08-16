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
    var game = _gameService.GetGame(id);

    return Ok(
        GameResponseMapper.ToResponse(game));
}
 [HttpPost("{id:guid}/moves")]
public ActionResult<GameStateResponse> MakeMove(
    Guid id,
    MoveRequest request)
{
    var game = _gameService.MakeMove(
        id,
        request.Player,
        request.Position);

    return Ok(
        GameResponseMapper.ToResponse(game));
}
[HttpPost("{id:guid}/undo")]
public ActionResult<GameStateResponse> Undo(Guid id)
{
    var game = _gameService.Undo(id);

    return Ok(
        GameResponseMapper.ToResponse(game));
}
[HttpPost("{id:guid}/reset")]
public ActionResult<GameStateResponse> Reset(Guid id)
{
    var game = _gameService.ResetGame(id);

    return Ok(
        GameResponseMapper.ToResponse(game));
}

}