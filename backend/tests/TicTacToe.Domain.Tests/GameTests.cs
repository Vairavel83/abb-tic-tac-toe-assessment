using TicTacToe.Domain.Enums;
using TicTacToe.Domain.Models;

namespace TicTacToe.Domain.Tests;

public class GameTests
{
    [Fact]
    public void NewGame_ShouldStartWithPlayerX()
    {
        // Arrange
        var game = new Game(GameMode.TwoPlayer);

        // Assert
        Assert.Equal(Player.X, game.CurrentPlayer);
    }
    [Fact]
public void MakeMove_ValidMove_ShouldPlacePlayerOnBoard()
{
    // Arrange
    var game = new Game(GameMode.TwoPlayer);

    // Act
    game.MakeMove(Player.X, 0);

    // Assert
    Assert.Equal(Player.X, game.Board.Cells[0]);
}
[Fact]
public void MakeMove_ValidMove_ShouldSwitchPlayer()
{
    // Arrange
    var game = new Game(GameMode.TwoPlayer);

    // Act
    game.MakeMove(Player.X, 0);

    // Assert
    Assert.Equal(Player.O, game.CurrentPlayer);
}
[Fact]
public void MakeMove_WrongPlayer_ShouldThrowException()
{
    // Arrange
    var game = new Game(GameMode.TwoPlayer);

    var action = () => game.MakeMove(Player.O, 0);

    var exception =
        Assert.Throws<InvalidOperationException>(action);

    Assert.Equal("It is X's turn.", exception.Message);

    Assert.Null(game.Board.Cells[0]);
    Assert.Equal(Player.X, game.CurrentPlayer);
}
[Fact]
public void MakeMove_OccupiedCell_ShouldThrowException()
{
    // Arrange
    var game = new Game(GameMode.TwoPlayer);

    game.MakeMove(Player.X, 0);

    // O tries the same cell
    var action = () => game.MakeMove(Player.O, 0);

    // Assert
    Assert.Throws<InvalidOperationException>(action);

    Assert.Equal(Player.X, game.Board.Cells[0]);

    Assert.Equal(Player.O, game.CurrentPlayer);

    Assert.Single(game.MoveHistory);
}
}