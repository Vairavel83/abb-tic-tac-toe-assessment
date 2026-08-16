using System.Collections.Concurrent;
using TicTacToe.Application.Interfaces;
using TicTacToe.Domain.Models;

namespace TicTacToe.Infrastructure.Repositories;

public class InMemoryGameRepository : IGameRepository
{
    private readonly ConcurrentDictionary<Guid, Game> _games = new();

    public Game Add(Game game)
    {
        if (!_games.TryAdd(game.Id, game))
        {
            throw new InvalidOperationException(
                $"Game with ID {game.Id} already exists.");
        }

        return game;
    }

    public Game? GetById(Guid gameId)
    {
        _games.TryGetValue(gameId, out var game);

        return game;
    }
}