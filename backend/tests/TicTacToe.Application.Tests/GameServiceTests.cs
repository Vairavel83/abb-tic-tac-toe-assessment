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
    [Fact]
public void CreateGame_ShouldCreateAndStoreGame()
{
    // Arrange
    var repository = new FakeGameRepository();
    var strategy = new FakeComputerMoveStrategy();

var service =
    new GameService(repository, strategy);

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

var service =
    new GameService(repository, strategy);

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

var service =
    new GameService(repository, strategy);

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

    var service =
        new GameService(repository, strategy);

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

    var service =
        new GameService(repository, strategy);

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

    var service =
        new GameService(repository, strategy);

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

    var service =
        new GameService(repository, strategy);

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

}