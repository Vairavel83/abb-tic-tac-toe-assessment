using TicTacToe.Application.Interfaces;
using TicTacToe.Application.Services;
using TicTacToe.Domain.Enums;
using TicTacToe.Domain.Models;
using TicTacToe.Domain.Services;

namespace TicTacToe.Application.Tests;

public class GameServiceTests
{
    private sealed class FakeGameRepository : IGameRepository
    {
        private readonly Dictionary<Guid, Game> _games = new();

        public Game Add(Game game)
        {
            _games.Add(game.Id, game);
            return game;
        }

        public Game? GetById(Guid gameId)
        {
            _games.TryGetValue(gameId, out var game);
            return game;
        }
    }
private sealed class FakeComputerMoveStrategy
    : IComputerMoveStrategy
{
    public int CellToReturn { get; set; } = 4;

    public int SelectMove(Board board)
    {
        return CellToReturn;
    }
}
private sealed class FakeScoreboardRepository
    : IScoreboardRepository
{
    private readonly Scoreboard _scoreboard = new();

    public Scoreboard Get()
    {
        return _scoreboard;
    }

    public void RecordWin(Player winner)
    {
        _scoreboard.RecordWin(winner);
    }

    public void RecordDraw()
    {
        _scoreboard.RecordDraw();
    }

    public Scoreboard Reset()
    {
        _scoreboard.Reset();

        return _scoreboard;
    }
}
    [Fact]
public void CreateGame_ShouldCreateAndStoreGame()
{
    // Arrange
    var repository = new FakeGameRepository();
    var strategy = new FakeComputerMoveStrategy();

var scoreboardRepository =
    new FakeScoreboardRepository();

var service =
    new GameService(
        repository,
        strategy,
        scoreboardRepository);

    // Act
    var game = service.CreateGame(GameMode.TwoPlayer);

    // Assert
    Assert.NotEqual(Guid.Empty, game.Id);
    Assert.Equal(GameMode.TwoPlayer, game.Mode);
    Assert.Equal(Player.X, game.CurrentPlayer);
    Assert.Equal(GameStatus.InProgress, game.Status);

    var storedGame = repository.GetById(game.Id);

    Assert.NotNull(storedGame);
    Assert.Same(game, storedGame);
}
[Fact]
public void GetGame_WhenGameExists_ShouldReturnGame()
{
    // Arrange
    var repository = new FakeGameRepository();
    var strategy = new FakeComputerMoveStrategy();

var scoreboardRepository =
    new FakeScoreboardRepository();

var service =
    new GameService(
        repository,
        strategy,
        scoreboardRepository);

    var createdGame =
        service.CreateGame(GameMode.Computer);

    // Act
    var result =
        service.GetGame(createdGame.Id);

    // Assert
    Assert.Same(createdGame, result);
}
[Fact]
public void GetGame_WhenGameDoesNotExist_ShouldThrowException()
{
    // Arrange
    var repository = new FakeGameRepository();
    var strategy = new FakeComputerMoveStrategy();

var scoreboardRepository =
    new FakeScoreboardRepository();

var service =
    new GameService(
        repository,
        strategy,
        scoreboardRepository);

    var unknownGameId = Guid.NewGuid();

    // Act
    var action =
        () => service.GetGame(unknownGameId);

    // Assert
    var exception =
        Assert.Throws<KeyNotFoundException>(action);

    Assert.Contains(
        unknownGameId.ToString(),
        exception.Message);
}

[Fact]
public void MakeMove_ComputerMode_ShouldApplyHumanAndComputerMoves()
{
    // Arrange
    var repository = new FakeGameRepository();

    var strategy = new FakeComputerMoveStrategy
    {
        CellToReturn = 4
    };

    var scoreboardRepository =
    new FakeScoreboardRepository();

var service =
    new GameService(
        repository,
        strategy,
        scoreboardRepository);

    var game =
        service.CreateGame(GameMode.Computer);

    // Act
    var result =
        service.MakeMove(
            game.Id,
            Player.X,
            0);

    // Assert
    Assert.Equal(Player.X, result.Board.Cells[0]);
    Assert.Equal(Player.O, result.Board.Cells[4]);

    Assert.Equal(2, result.MoveHistory.Count);

    Assert.Equal(Player.X, result.CurrentPlayer);
}
[Fact]
public void MakeMove_WhenHumanWins_ShouldNotMakeComputerMove()
{
    // Arrange
    var repository = new FakeGameRepository();

    var strategy = new FakeComputerMoveStrategy
    {
        CellToReturn = 8
    };

   var scoreboardRepository =
    new FakeScoreboardRepository();

var service =
    new GameService(
        repository,
        strategy,
        scoreboardRepository);

    var game =
        service.CreateGame(GameMode.Computer);

    /*
       We prepare:

       X | X | .
       ---------
       O | O | .
       ---------
       . | . | .

       We use Game directly only for arranging the state.
    */

    game.MakeMove(Player.X, 0);
    game.MakeMove(Player.O, 3);

    game.MakeMove(Player.X, 1);
    game.MakeMove(Player.O, 4);

    // Act
    service.MakeMove(
        game.Id,
        Player.X,
        2);

    // Assert
    Assert.Equal(GameStatus.Won, game.Status);
    Assert.Equal(Player.X, game.Winner);

    Assert.Equal(5, game.MoveHistory.Count);

    Assert.Null(game.Board.Cells[8]);
}

[Fact]
public void MakeMove_ComputerMode_WhenHumanSubmitsO_ShouldRejectMove()
{
    // Arrange
    var repository = new FakeGameRepository();
    var strategy = new FakeComputerMoveStrategy();

    var scoreboardRepository =
    new FakeScoreboardRepository();

var service =
    new GameService(
        repository,
        strategy,
        scoreboardRepository);

    var game =
        service.CreateGame(GameMode.Computer);

    // Act
    var action =
        () => service.MakeMove(
            game.Id,
            Player.O,
            0);

    // Assert
    var exception =
        Assert.Throws<InvalidOperationException>(action);

    Assert.Equal(
        "In computer mode, the human player must be X.",
        exception.Message);

    Assert.Empty(game.MoveHistory);
}

[Fact]
public void Undo_ComputerMode_ShouldRemoveHumanAndComputerMoves()
{
    // Arrange
    var repository = new FakeGameRepository();

    var strategy = new FakeComputerMoveStrategy
    {
        CellToReturn = 4
    };

   var scoreboardRepository =
    new FakeScoreboardRepository();

var service =
    new GameService(
        repository,
        strategy,
        scoreboardRepository);

    var game =
        service.CreateGame(GameMode.Computer);

    service.MakeMove(
        game.Id,
        Player.X,
        0);

    // Before undo:
    // X at 0
    // O at 4
    Assert.Equal(2, game.MoveHistory.Count);

    // Act
    service.Undo(game.Id);

    // Assert
    Assert.Null(game.Board.Cells[0]);
    Assert.Null(game.Board.Cells[4]);

    Assert.Empty(game.MoveHistory);

    Assert.Equal(Player.X, game.CurrentPlayer);
    Assert.Equal(GameStatus.InProgress, game.Status);
}
[Fact]
public void MakeMove_WhenXWins_ShouldIncrementXWins()
{
    // Arrange
    var repository = new FakeGameRepository();
    var strategy = new FakeComputerMoveStrategy();
    var scoreboardRepository = new FakeScoreboardRepository();

    var service = new GameService(
        repository,
        strategy,
        scoreboardRepository);

    var game = service.CreateGame(GameMode.TwoPlayer);

    // Act
    service.MakeMove(game.Id, Player.X, 0);
    service.MakeMove(game.Id, Player.O, 3);

    service.MakeMove(game.Id, Player.X, 1);
    service.MakeMove(game.Id, Player.O, 4);

    service.MakeMove(game.Id, Player.X, 2);

    // Assert
    var scoreboard = scoreboardRepository.Get();

    Assert.Equal(GameStatus.Won, game.Status);
    Assert.Equal(Player.X, game.Winner);

    Assert.Equal(1, scoreboard.XWins);
    Assert.Equal(0, scoreboard.OWins);
    Assert.Equal(0, scoreboard.Draws);

    Assert.True(game.ScoreRecorded);
}
[Fact]
public void MakeMove_AfterGameAlreadyCompleted_ShouldNotIncrementScoreAgain()
{
    // Arrange
    var repository = new FakeGameRepository();
    var strategy = new FakeComputerMoveStrategy();
    var scoreboardRepository = new FakeScoreboardRepository();

    var service = new GameService(
        repository,
        strategy,
        scoreboardRepository);

    var game = service.CreateGame(GameMode.TwoPlayer);

    // X wins
    service.MakeMove(game.Id, Player.X, 0);
    service.MakeMove(game.Id, Player.O, 3);

    service.MakeMove(game.Id, Player.X, 1);
    service.MakeMove(game.Id, Player.O, 4);

    service.MakeMove(game.Id, Player.X, 2);

    Assert.Equal(1, scoreboardRepository.Get().XWins);

    // Act
    var action = () =>
        service.MakeMove(game.Id, Player.O, 5);

    // Assert
    Assert.Throws<InvalidOperationException>(action);

    Assert.Equal(1, scoreboardRepository.Get().XWins);
    Assert.True(game.ScoreRecorded);
}
[Fact]
public void ResetGame_ShouldClearGameButKeepScoreboard()
{
    // Arrange
    var repository = new FakeGameRepository();
    var strategy = new FakeComputerMoveStrategy();
    var scoreboardRepository = new FakeScoreboardRepository();

    var service = new GameService(
        repository,
        strategy,
        scoreboardRepository);

    var game = service.CreateGame(GameMode.TwoPlayer);

    // Complete first round - X wins
    service.MakeMove(game.Id, Player.X, 0);
    service.MakeMove(game.Id, Player.O, 3);

    service.MakeMove(game.Id, Player.X, 1);
    service.MakeMove(game.Id, Player.O, 4);

    service.MakeMove(game.Id, Player.X, 2);

    Assert.Equal(1, scoreboardRepository.Get().XWins);
    Assert.True(game.ScoreRecorded);

    // Act
    service.ResetGame(game.Id);

    // Assert - game reset
    Assert.All(
        game.Board.Cells,
        cell => Assert.Null(cell));

    Assert.Empty(game.MoveHistory);

    Assert.Equal(Player.X, game.CurrentPlayer);
    Assert.Equal(GameStatus.InProgress, game.Status);

    Assert.Null(game.Winner);
    Assert.Empty(game.WinningCells);

    Assert.False(game.ScoreRecorded);

    // Scoreboard must remain unchanged
    Assert.Equal(1, scoreboardRepository.Get().XWins);
    Assert.Equal(0, scoreboardRepository.Get().OWins);
    Assert.Equal(0, scoreboardRepository.Get().Draws);
    // Play another round after reset
service.MakeMove(game.Id, Player.X, 0);
service.MakeMove(game.Id, Player.O, 3);

service.MakeMove(game.Id, Player.X, 1);
service.MakeMove(game.Id, Player.O, 4);

service.MakeMove(game.Id, Player.X, 2);

// Same game resource, new completed round
Assert.Equal(2, scoreboardRepository.Get().XWins);
Assert.True(game.ScoreRecorded);
}

}