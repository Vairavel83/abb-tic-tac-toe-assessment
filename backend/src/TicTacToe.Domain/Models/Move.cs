using TicTacToe.Domain.Enums;

namespace TicTacToe.Domain.Models;
public class Move
{
    public int CellIndex { get; set; }
    public Player Player { get; set; }
    public int MoveNumber { get; set; }
    public int row => CellIndex / 3;
    public int column => CellIndex % 3;
    public Move(int cellIndex, Player player, int moveNumber)
    {
        CellIndex = cellIndex;
        Player = player;
        MoveNumber = moveNumber;
    }
}