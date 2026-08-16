using TicTacToe.Domain.Models;

namespace TicTacToe.Application.Interfaces;

public interface IGameRepository
{
    Game Add(Game game);

    Game? GetById(Guid gameId);
}