using TicTacToe.Application.Interfaces;
using TicTacToe.Application.Services;
using TicTacToe.Domain.Enums;
using TicTacToe.Domain.Models;

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

    [Fact]
public void CreateGame_ShouldCreateAndStoreGame()
{
    // Arrange
    var repository = new FakeGameRepository();
    var service = new GameService(repository);

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
    var service = new GameService(repository);

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
    var service = new GameService(repository);

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

}