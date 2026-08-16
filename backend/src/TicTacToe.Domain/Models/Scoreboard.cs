using TicTacToe.Domain.Enums;

namespace TicTacToe.Domain.Models;

public class Scoreboard
{
    public int XWins { get; private set; }

    public int OWins { get; private set; }

    public int Draws { get; private set; }

    public void RecordWin(Player winner)
    {
        if (winner == Player.X)
        {
            XWins++;
        }
        else
        {
            OWins++;
        }
    }

    public void RecordDraw()
    {
        Draws++;
    }

    public void Reset()
    {
        XWins = 0;
        OWins = 0;
        Draws = 0;
    }
}