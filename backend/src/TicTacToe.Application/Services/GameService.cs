using TicTacToe.Application.Interfaces;
using TicTacToe.Domain.Enums;
using TicTacToe.Domain.Models;
using TicTacToe.Domain.Services;

namespace TicTacToe.Application.Services;

public class GameService
{
    private readonly IGameRepository _gameRepository;
    private readonly IComputerMoveStrategy _computerMoveStrategy;

    public GameService(IGameRepository gameRepository, IComputerMoveStrategy computerMoveStrategy)
    {
        _gameRepository = gameRepository;
        _computerMoveStrategy = computerMoveStrategy;
    }
    public Game MakeMove(
    Guid gameId,
    Player player,
    int cellIndex)
{
    var game = GetGame(gameId);

    if (game.Mode == GameMode.TwoPlayer)
    {
        game.MakeMove(player, cellIndex);

        return game;
    }

    if (player != Player.X)
    {
        throw new InvalidOperationException(
            "In computer mode, the human player must be X.");
    }

    // Human move
    game.MakeMove(Player.X, cellIndex);

    // Do not allow the computer to move if
    // the human move already completed the game.
    if (game.Status != GameStatus.InProgress)
    {
        return game;
    }

    // Computer chooses O's move.
    var computerCell =
        _computerMoveStrategy.SelectMove(game.Board);

    game.MakeMove(Player.O, computerCell);

    return game;
}
    

    public Game CreateGame(GameMode mode)
    {
        var game = new Game(mode);

        return _gameRepository.Add(game);
    }

    public Game GetGame(Guid gameId)
    {
        var game = _gameRepository.GetById(gameId);

        if (game is null)
        {
            throw new KeyNotFoundException(
                $"Game with ID {gameId} was not found.");
        }

        return game;
    }
}