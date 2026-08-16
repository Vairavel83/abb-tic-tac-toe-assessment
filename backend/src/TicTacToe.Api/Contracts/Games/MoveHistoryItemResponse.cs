using TicTacToe.Domain.Enums;

namespace TicTacToe.Api.Contracts.Games;

public sealed class MoveHistoryItemResponse
{
    public int MoveNumber { get; init; }

    public Player Player { get; init; }

    public int Position { get; init; }

    public int Row { get; init; }

    public int Column { get; init; }
}