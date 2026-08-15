using TicTacToe.Domain.Enums;
using TicTacToe.Domain.Models;
using TicTacToe.Domain.Services;

namespace TicTacToe.Domain.Tests;

public class ComputerStrategyTests
{
    [Fact]
    public void SelectMove_WhenComputerCanWin_ShouldChooseWinningCell()
    {
        // Arrange
        var board = new Board();

        board.Place(0, Player.O);
        board.Place(1, Player.O);

        board.Place(3, Player.X);
        board.Place(4, Player.X);

        var strategy = new BasicComputerMoveStrategy();

        // Act
        var selectedCell = strategy.SelectMove(board);

        // Assert
        Assert.Equal(2, selectedCell);
    }
    [Fact]
public void SelectMove_WhenHumanCanWin_ShouldBlockWinningCell()
{
    // Arrange
    var board = new Board();

    board.Place(0, Player.X);
    board.Place(1, Player.X);

    board.Place(4, Player.O);

    var strategy = new BasicComputerMoveStrategy();

    // Act
    var selectedCell = strategy.SelectMove(board);

    // Assert
    Assert.Equal(2, selectedCell);
}
[Fact]
public void SelectMove_WhenNoWinOrBlock_ShouldTakeCenter()
{
    // Arrange
    var board = new Board();

    board.Place(0, Player.X);

    var strategy = new BasicComputerMoveStrategy();

    // Act
    var selectedCell = strategy.SelectMove(board);

    // Assert
    Assert.Equal(4, selectedCell);
}
[Fact]
public void SelectMove_WhenCenterUnavailable_ShouldTakeCorner()
{
    // Arrange
    var board = new Board();

    board.Place(4, Player.X);

    var strategy = new BasicComputerMoveStrategy();

    // Act
    var selectedCell = strategy.SelectMove(board);

    // Assert
    Assert.Equal(0, selectedCell);
}
[Fact]
public void SelectMove_WhenOnlyEdgeCellsAvailable_ShouldTakeAvailableCell()
{
    // Arrange
    var board = new Board();

    board.Place(0, Player.X);
    board.Place(2, Player.O);
    board.Place(4, Player.X);
    board.Place(6, Player.O);
    board.Place(8, Player.X);

    var strategy = new BasicComputerMoveStrategy();

    // Act
    var selectedCell = strategy.SelectMove(board);

    // Assert
    Assert.Equal(1, selectedCell);
}
}