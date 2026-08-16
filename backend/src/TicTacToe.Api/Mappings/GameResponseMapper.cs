using TicTacToe.Api.Contracts.Games;
using TicTacToe.Domain.Models;

namespace TicTacToe.Api.Mappings;

public static class GameResponseMapper
{
    public static GameStateResponse ToResponse(Game game)
    {
        return new GameStateResponse
        {
            GameId = game.Id,

            Board = game.Board.Cells.ToArray(),

            CurrentPlayer = game.CurrentPlayer,

            Mode = game.Mode,

            Status = game.Status,

            Winner = game.Winner,

            WinningCells = game.WinningCells.ToArray(),

            MoveHistory = game.MoveHistory
                .Select(move => new MoveHistoryItemResponse
                {
                    MoveNumber = move.MoveNumber,
                    Player = move.Player,
                    Position = move.CellIndex,
                    Row = (move.CellIndex / 3) + 1,
                    Column = (move.CellIndex % 3) + 1
                })
                .ToArray()
        };
    }
}