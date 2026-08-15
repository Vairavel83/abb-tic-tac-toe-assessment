using TicTacToe.Domain.Enums;
using TicTacToe.Domain.Models;

namespace TicTacToe.Domain.Services;

public class BasicComputerMoveStrategy : IComputerMoveStrategy
{
    private readonly GameEvaluator _evaluator = new();

    private static readonly int[] Corners =
    {
        0, 2, 6, 8
    };

    public int SelectMove(Board board)
    {
        var availableCells = board.GetAvailableCells();

        if (availableCells.Count == 0)
        {
            throw new InvalidOperationException(
                "There are no available cells.");
        }

        // 1. If O can win, take the winning move.
        var winningMove = FindWinningMove(board, Player.O);

        if (winningMove.HasValue)
        {
            return winningMove.Value;
        }

        // 2. If X can win next, block X.
        var blockingMove = FindWinningMove(board, Player.X);

        if (blockingMove.HasValue)
        {
            return blockingMove.Value;
        }

        // 3. Take center.
        if (board.IsEmpty(4))
        {
            return 4;
        }

        // 4. Take a corner.
        foreach (var corner in Corners)
        {
            if (board.IsEmpty(corner))
            {
                return corner;
            }
        }

        // 5. Take any available cell.
        return availableCells[0];
    }

    private int? FindWinningMove(
        Board board,
        Player player)
    {
        foreach (var cell in board.GetAvailableCells())
        {
            var simulatedBoard = board.Clone();

            simulatedBoard.Place(cell, player);

            var evaluation =
                _evaluator.Evaluate(simulatedBoard);

            if (evaluation.Status == GameStatus.Won &&
                evaluation.Winner == player)
            {
                return cell;
            }
        }

        return null;
    }
}