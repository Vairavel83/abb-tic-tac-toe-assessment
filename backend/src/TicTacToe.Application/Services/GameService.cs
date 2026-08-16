using TicTacToe.Application.Interfaces;
using TicTacToe.Domain.Enums;
using TicTacToe.Domain.Models;

namespace TicTacToe.Application.Services;

public class GameService
{
    private readonly IGameRepository _gameRepository;

    public GameService(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public Game CreateGame(GameMode mode)
    {
        var game = new Game(mode);

        return _gameRepository.Add(game);
    }

    public Game GetGame(Guid gameId)
    {
        var game = _gameRepository.GetById(gameId);

        if (game is null)
        {
            throw new KeyNotFoundException(
                $"Game with ID {gameId} was not found.");
        }

        return game;
    }
}