using TicTacToe.Domain.Enums;
using TicTacToe.Domain.Services;

namespace TicTacToe.Domain.Models;

public class Game
{
    private readonly GameEvaluator _evaluator = new();

    private readonly List<Move> _moveHistory = new();

    public Guid Id { get; } = Guid.NewGuid();

    public Board Board { get; } = new();

    public GameMode Mode { get; }

    public Player CurrentPlayer { get; private set; } = Player.X;

    public GameStatus Status { get; private set; } = GameStatus.InProgress;

    public Player? Winner { get; private set; }

    public IReadOnlyList<int> WinningCells { get; private set; }
        = Array.Empty<int>();

    public IReadOnlyList<Move> MoveHistory => _moveHistory;

    public Game(GameMode mode)
    {
        Mode = mode;
    }

    public void MakeMove(Player player, int cellIndex)
{
    if (Status != GameStatus.InProgress)
    {
        throw new InvalidOperationException(
            "Cannot make a move after the game is completed.");
    }

    if (!Board.IsValidCell(cellIndex))
    {
        throw new ArgumentOutOfRangeException(
            nameof(cellIndex),
            "Cell index must be between 0 and 8.");
    }

    if (player != CurrentPlayer)
    {
        throw new InvalidOperationException(
            $"It is {CurrentPlayer}'s turn.");
    }

    if (!Board.IsEmpty(cellIndex))
    {
        throw new InvalidOperationException(
            $"Cell {cellIndex} is already occupied.");
    }

    Board.Place(cellIndex, player);

    _moveHistory.Add(
        new Move(
            _moveHistory.Count + 1,
            player,
            cellIndex));

    EvaluateGame();

    if (Status == GameStatus.InProgress)
    {
        SwitchPlayer();
    }

}
private void EvaluateGame()
{
    var evaluation = _evaluator.Evaluate(Board);

    Status = evaluation.Status;
    Winner = evaluation.Winner;
    WinningCells = evaluation.WinningCells;
}

private void SwitchPlayer()
{
    CurrentPlayer =
        CurrentPlayer == Player.X
            ? Player.O
            : Player.X;
}
}