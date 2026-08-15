using TicTacToe.Domain.Enums;
using TicTacToe.Domain.Models;

namespace TicTacToe.Domain.Services;

public class GameEvaluator
{
    private static readonly int[][] WinningCombinations =
    [
        [0, 1, 2],
        [3, 4, 5],
        [6, 7, 8],

        [0, 3, 6],
        [1, 4, 7],
        [2, 5, 8],

        [0, 4, 8],
        [2, 4, 6]
    ];

    public GameEvaluation Evaluate(Board board)
    {
        foreach (var combination in WinningCombinations)
        {
            var first = board.Cells[combination[0]];
            var second = board.Cells[combination[1]];
            var third = board.Cells[combination[2]];

            if (first is not null &&
                first == second &&
                second == third)
            {
                return new GameEvaluation(
                    GameStatus.Won,
                    first,
                    combination);
            }
        }

        if (board.IsFull())
        {
            return new GameEvaluation(GameStatus.Draw);
        }

        return new GameEvaluation(GameStatus.InProgress);
    }
}