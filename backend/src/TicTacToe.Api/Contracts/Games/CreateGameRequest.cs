using TicTacToe.Domain.Enums;

namespace TicTacToe.Api.Contracts.Games;

public sealed class CreateGameRequest
{
    public GameMode Mode { get; init; }
}