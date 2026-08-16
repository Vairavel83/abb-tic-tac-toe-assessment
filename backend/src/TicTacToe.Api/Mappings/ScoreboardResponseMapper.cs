using TicTacToe.Api.Contracts.Scoreboard;
using TicTacToe.Domain.Models;

namespace TicTacToe.Api.Mappings;

public static class ScoreboardResponseMapper
{
    public static ScoreboardResponse ToResponse(
        Scoreboard scoreboard)
    {
        return new ScoreboardResponse
        {
            XWins = scoreboard.XWins,
            OWins = scoreboard.OWins,
            Draws = scoreboard.Draws
        };
    }
}