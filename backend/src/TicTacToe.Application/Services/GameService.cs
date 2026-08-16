using TicTacToe.Application.Interfaces;
using TicTacToe.Domain.Enums;
using TicTacToe.Domain.Models;
using TicTacToe.Domain.Services;

namespace TicTacToe.Application.Services;

public class GameService
{
    private readonly IGameRepository _gameRepository;
    private readonly IComputerMoveStrategy _computerMoveStrategy;

private readonly IScoreboardRepository _scoreboardRepository;
    public GameService(
    IGameRepository gameRepository,
    IComputerMoveStrategy computerMoveStrategy,
    IScoreboardRepository scoreboardRepository)
{
    _gameRepository = gameRepository;
    _computerMoveStrategy = computerMoveStrategy;
    _scoreboardRepository = scoreboardRepository;
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
        RecordScoreIfCompleted(game);

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
        RecordScoreIfCompleted(game);
        return game;
    }

    // Computer chooses O's move.
    var computerCell =
        _computerMoveStrategy.SelectMove(game.Board);

    game.MakeMove(Player.O, computerCell);
    RecordScoreIfCompleted(game);

    return game;
}
public Game Undo(Guid gameId)
{
    var game = GetGame(gameId);

    if (game.Mode == GameMode.TwoPlayer)
    {
        game.UndoLastMove();
        return game;
    }

    // Computer mode:
    // remove computer O move first
    game.UndoLastMove();

    // remove previous human X move
    if (game.MoveHistory.Count > 0)
    {
        game.UndoLastMove();
    }

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

    private void RecordScoreIfCompleted(Game game)
{
    if (game.Status == GameStatus.InProgress ||
        game.ScoreRecorded)
    {
        return;
    }

    if (game.Status == GameStatus.Won &&
        game.Winner.HasValue)
    {
        _scoreboardRepository.RecordWin(
            game.Winner.Value);
    }
    else if (game.Status == GameStatus.Draw)
    {
        _scoreboardRepository.RecordDraw();
    }

    game.MarkScoreRecorded();
}
public Game ResetGame(Guid gameId)
{
    var game = GetGame(gameId);

    game.Reset();

    return game;
}
}