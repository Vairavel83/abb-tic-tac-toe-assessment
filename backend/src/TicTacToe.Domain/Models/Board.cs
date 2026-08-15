using TicTacToe.Domain.Enums;

namespace TicTacToe.Domain.Models;
public class Board
{
    //public Player[,] Cells { get; set; } = new Player[3, 3];
    private readonly Player?[] _cells   = new Player?[9];
    public IReadOnlyList<Player?> Cells => _cells;
    public bool IsValidCell(int cellIndex)
    {
        return cellIndex >= 0 && cellIndex < 9;
    }
    public bool IsEmpty(int cellIndex)
    {
        return IsValidCell(cellIndex) && _cells[cellIndex] is null;
    }
    public void Place(int cellIndex, Player player)
    {
        if (!IsValidCell(cellIndex))
            throw new ArgumentOutOfRangeException(nameof(cellIndex), "Cell index must be between 0 and 8.");
        if (!IsEmpty(cellIndex))
            throw new InvalidOperationException("Cell is already occupied.");
        _cells[cellIndex] = player;
    }
    public void Clear(int cellIndex)
    {
        if (!IsValidCell(cellIndex))
            throw new ArgumentOutOfRangeException(nameof(cellIndex), "Cell index must be between 0 and 8.");
        _cells[cellIndex] = null;
    }
    public void Reset()
    {
     Array.Clear(_cells);
    }
    public IReadOnlyList<int> GetAvailableCells()
    {
         return Enumerable.Range(0, 9)
            .Where(IsEmpty)
            .ToList();
    }
    public bool IsFull()
    {
        return _cells.All(cell => cell is not null);
    }
}