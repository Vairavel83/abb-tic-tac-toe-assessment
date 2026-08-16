using TicTacToe.Application.Interfaces;
using TicTacToe.Domain.Models;

namespace TicTacToe.Application.Services;

public class ScoreboardService
{
    private readonly IScoreboardRepository _scoreboardRepository;

    public ScoreboardService(
        IScoreboardRepository scoreboardRepository)
    {
        _scoreboardRepository = scoreboardRepository;
    }

    public Scoreboard GetScoreboard()
    {
        return _scoreboardRepository.Get();
    }

    public Scoreboard ResetScoreboard()
    {
        return _scoreboardRepository.Reset();
    }
}