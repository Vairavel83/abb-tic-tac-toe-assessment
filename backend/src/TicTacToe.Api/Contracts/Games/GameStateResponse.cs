using TicTacToe.Domain.Enums;

namespace TicTacToe.Api.Contracts.Games;

public sealed class GameStateResponse
{
    public Guid GameId { get; init; }

    public IReadOnlyList<Player?> Board { get; init; }
        = Array.Empty<Player?>();

    public Player CurrentPlayer { get; init; }

    public GameMode Mode { get; init; }

    public GameStatus Status { get; init; }

    public Player? Winner { get; init; }

    public IReadOnlyList<int> WinningCells { get; init; }
        = Array.Empty<int>();

    public IReadOnlyList<MoveHistoryItemResponse> MoveHistory { get; init; }
        = Array.Empty<MoveHistoryItemResponse>();
}