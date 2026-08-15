using TicTacToe.Domain.Enums;

namespace TicTacToe.Domain.Models;

public class GameEvaluation
{
    public GameStatus Status { get; }
    public Player? Winner { get; }
    public IReadOnlyList<int> WinningCells { get; }

    public GameEvaluation(
        GameStatus status,
        Player? winner = null,
        IReadOnlyList<int>? winningCells = null)
    {
        Status = status;
        Winner = winner;
        WinningCells = winningCells ?? Array.Empty<int>();
    }
}