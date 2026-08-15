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
[Fact]
public void MakeMove_RowCompleted_ShouldSetGameAsWon()
{
    // Arrange
    var game = new Game(GameMode.TwoPlayer);

    // Act
    game.MakeMove(Player.X, 0);
    game.MakeMove(Player.O, 3);

    game.MakeMove(Player.X, 1);
    game.MakeMove(Player.O, 4);

    game.MakeMove(Player.X, 2);

    // Assert
    Assert.Equal(GameStatus.Won, game.Status);
    Assert.Equal(Player.X, game.Winner);

    Assert.Equal(
        new[] { 0, 1, 2 },
        game.WinningCells);
}
[Fact]
public void MakeMove_ColumnCompleted_ShouldSetGameAsWon()
{
    // Arrange
    var game = new Game(GameMode.TwoPlayer);

    // Act
    game.MakeMove(Player.X, 0);
    game.MakeMove(Player.O, 1);

    game.MakeMove(Player.X, 3);
    game.MakeMove(Player.O, 2);

    game.MakeMove(Player.X, 6);

    // Assert
    Assert.Equal(GameStatus.Won, game.Status);
    Assert.Equal(Player.X, game.Winner);

    Assert.Equal(
        new[] { 0, 3, 6 },
        game.WinningCells);
}
[Fact]
public void MakeMove_DiagonalCompleted_ShouldSetGameAsWon()
{
    // Arrange
    var game = new Game(GameMode.TwoPlayer);

    // Act
    game.MakeMove(Player.X, 0);
    game.MakeMove(Player.O, 1);

    game.MakeMove(Player.X, 4);
    game.MakeMove(Player.O, 2);

    game.MakeMove(Player.X, 8);

    // Assert
    Assert.Equal(GameStatus.Won, game.Status);
    Assert.Equal(Player.X, game.Winner);

    Assert.Equal(
        new[] { 0, 4, 8 },
        game.WinningCells);
}
[Fact]
public void MakeMove_BoardFullWithoutWinner_ShouldSetGameAsDraw()
{
    // Arrange
    var game = new Game(GameMode.TwoPlayer);

    // Act
    game.MakeMove(Player.X, 0);
    game.MakeMove(Player.O, 1);

    game.MakeMove(Player.X, 2);
    game.MakeMove(Player.O, 4);

    game.MakeMove(Player.X, 3);
    game.MakeMove(Player.O, 5);

    game.MakeMove(Player.X, 7);
    game.MakeMove(Player.O, 6);

    game.MakeMove(Player.X, 8);

    // Assert
    Assert.Equal(GameStatus.Draw, game.Status);
    Assert.Null(game.Winner);
    Assert.Empty(game.WinningCells);
}
[Fact]
public void MakeMove_AfterGameCompleted_ShouldThrowException()
{
    // Arrange
    var game = new Game(GameMode.TwoPlayer);

    game.MakeMove(Player.X, 0);
    game.MakeMove(Player.O, 3);

    game.MakeMove(Player.X, 1);
    game.MakeMove(Player.O, 4);

    game.MakeMove(Player.X, 2); // X wins

    // Act
    var action = () => game.MakeMove(Player.O, 5);

    // Assert
    var exception =
        Assert.Throws<InvalidOperationException>(action);

    Assert.Equal(
        "Cannot make a move after the game is completed.",
        exception.Message);

    Assert.Null(game.Board.Cells[5]);
    Assert.Equal(5, game.MoveHistory.Count);
}
[Fact]
public void Reset_ShouldClearGameStateAndStartWithPlayerX()
{
    // Arrange
    var game = new Game(GameMode.TwoPlayer);

    game.MakeMove(Player.X, 0);
    game.MakeMove(Player.O, 4);

    // Act
    game.Reset();

    // Assert
    Assert.All(game.Board.Cells, cell => Assert.Null(cell));

    Assert.Empty(game.MoveHistory);

    Assert.Equal(Player.X, game.CurrentPlayer);

    Assert.Equal(GameStatus.InProgress, game.Status);

    Assert.Null(game.Winner);

    Assert.Empty(game.WinningCells);
}
[Fact]
public void UndoLastMove_TwoPlayerMode_ShouldRemoveLatestMove()
{
    // Arrange
    var game = new Game(GameMode.TwoPlayer);

    game.MakeMove(Player.X, 0);
    game.MakeMove(Player.O, 4);
Assert.Equal(2, game.MoveHistory.Count);
Assert.Equal(Player.O, game.MoveHistory[^1].Player);
Assert.Equal(4, game.MoveHistory[^1].CellIndex);

    // Act
    game.UndoLastMove();

    // Assert
    Assert.Equal(Player.X, game.Board.Cells[0]);
    Assert.Null(game.Board.Cells[4]);

    Assert.Single(game.MoveHistory);

    Assert.Equal(Player.O, game.CurrentPlayer);

    Assert.Equal(GameStatus.InProgress, game.Status);
    Assert.Null(game.Winner);
    Assert.Empty(game.WinningCells);
}
}