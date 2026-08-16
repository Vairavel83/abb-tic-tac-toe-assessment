using TicTacToe.Domain.Enums;

namespace TicTacToe.Api.Contracts.Games;

public sealed class MoveRequest
{
    public Player Player { get; init; }

    public int Position { get; init; }
}