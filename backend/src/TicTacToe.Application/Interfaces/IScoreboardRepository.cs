using TicTacToe.Domain.Enums;
using TicTacToe.Domain.Models;

namespace TicTacToe.Application.Interfaces;

public interface IScoreboardRepository
{
    Scoreboard Get();

    void RecordWin(Player winner);

    void RecordDraw();

    Scoreboard Reset();
}