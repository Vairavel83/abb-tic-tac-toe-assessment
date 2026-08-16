export type Player = 'X' | 'O';

export type GameMode =
  | 'TwoPlayer'
  | 'Computer';

export type GameStatus =
  | 'InProgress'
  | 'Won'
  | 'Draw';

export interface MoveHistoryItem {
  moveNumber: number;
  player: Player;
  position: number;
  row: number;
  column: number;
}

export interface GameState {
  gameId: string;

  board: (Player | null)[];

  currentPlayer: Player;

  mode: GameMode;

  status: GameStatus;

  winner: Player | null;

  winningCells: number[];

  moveHistory: MoveHistoryItem[];
}

export interface CreateGameRequest {
  mode: GameMode;
}

export interface MoveRequest {
  player: Player;
  position: number;
}