namespace TicTacToe.Api.Contracts.Scoreboard;

public sealed class ScoreboardResponse
{
    public int XWins { get; init; }

    public int OWins { get; init; }

    public int Draws { get; init; }
}