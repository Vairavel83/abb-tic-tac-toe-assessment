using TicTacToe.Application.Interfaces;
using TicTacToe.Application.Services;
using TicTacToe.Domain.Enums;
using TicTacToe.Domain.Models;

namespace TicTacToe.Application.Tests;

public class ScoreboardServiceTests
{
    private sealed class FakeScoreboardRepository
        : IScoreboardRepository
    {
        private readonly Scoreboard _scoreboard = new();

        public Scoreboard Get()
        {
            return _scoreboard;
        }

        public void RecordWin(Player winner)
        {
            _scoreboard.RecordWin(winner);
        }

        public void RecordDraw()
        {
            _scoreboard.RecordDraw();
        }

        public Scoreboard Reset()
        {
            _scoreboard.Reset();
            return _scoreboard;
        }
    }

    [Fact]
    public void ResetScoreboard_ShouldResetAllValuesToZero()
    {
        // Arrange
        var repository = new FakeScoreboardRepository();

        repository.RecordWin(Player.X);
        repository.RecordWin(Player.X);
        repository.RecordWin(Player.O);
        repository.RecordDraw();

        var service =
            new ScoreboardService(repository);

        // Act
        var result = service.ResetScoreboard();

        // Assert
        Assert.Equal(0, result.XWins);
        Assert.Equal(0, result.OWins);
        Assert.Equal(0, result.Draws);
    }
    [Fact]
public void GetScoreboard_ShouldReturnCurrentScoreboard()
{
    // Arrange
    var repository = new FakeScoreboardRepository();

    repository.RecordWin(Player.X);
    repository.RecordDraw();

    var service =
        new ScoreboardService(repository);

    // Act
    var result = service.GetScoreboard();

    // Assert
    Assert.Equal(1, result.XWins);
    Assert.Equal(0, result.OWins);
    Assert.Equal(1, result.Draws);
}
}
