using TicTacToe.Domain.Models;

namespace TicTacToe.Domain.Services;

public interface IComputerMoveStrategy
{
    int SelectMove(Board board);
}