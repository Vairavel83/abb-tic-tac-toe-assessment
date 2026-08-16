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
    public bool ScoreRecorded { get; private set; }

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
        cellIndex: cellIndex,
        player: player,
        moveNumber: _moveHistory.Count + 1));

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
public void Reset()
{
    Board.Reset();

    _moveHistory.Clear();

    CurrentPlayer = Player.X;

    Status = GameStatus.InProgress;

    Winner = null;

    WinningCells = Array.Empty<int>();
    ScoreRecorded = false;
}
public void UndoLastMove()
{
    if (Status != GameStatus.InProgress)
    {
        throw new InvalidOperationException(
            "Undo is not allowed after game completion.");
    }

    if (_moveHistory.Count == 0)
    {
        throw new InvalidOperationException(
            "There are no moves to undo.");
    }

    var lastMove = _moveHistory[^1];

    Board.Clear(lastMove.CellIndex);

    _moveHistory.RemoveAt(_moveHistory.Count - 1);

    CurrentPlayer = lastMove.Player;

    EvaluateGame();
}
public void MarkScoreRecorded()
{
    ScoreRecorded = true;
}
}