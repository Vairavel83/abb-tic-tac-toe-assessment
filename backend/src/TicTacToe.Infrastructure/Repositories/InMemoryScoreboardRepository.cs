using TicTacToe.Application.Interfaces;
using TicTacToe.Domain.Enums;
using TicTacToe.Domain.Models;

namespace TicTacToe.Infrastructure.Repositories;

public class InMemoryScoreboardRepository : IScoreboardRepository
{
    private readonly Scoreboard _scoreboard = new();

    private readonly object _lock = new();

    public Scoreboard Get()
    {
        return _scoreboard;
    }

    public void RecordWin(Player winner)
    {
        lock (_lock)
        {
            _scoreboard.RecordWin(winner);
        }
    }

    public void RecordDraw()
    {
        lock (_lock)
        {
            _scoreboard.RecordDraw();
        }
    }

    public Scoreboard Reset()
    {
        lock (_lock)
        {
            _scoreboard.Reset();

            return _scoreboard;
        }
    }
}