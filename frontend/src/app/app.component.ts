import { Component, inject, OnInit } from '@angular/core';
import { finalize } from 'rxjs';

import { GameApiService } from './core/services/game-api.service';
import {
  GameMode,
  GameState,
  Player
} from './core/models/game.models';
import { Scoreboard } from './core/models/scoreboard.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  private readonly gameApi = inject(GameApiService);

  game: GameState | null = null;

  scoreboard: Scoreboard = {
    xWins: 0,
    oWins: 0,
    draws: 0
  };

  isLoading = false;
  errorMessage = '';

  ngOnInit(): void {
    this.loadScoreboard();
  }

  startGame(mode: GameMode): void {
    this.errorMessage = '';
    this.isLoading = true;

    this.gameApi
      .createGame(mode)
      .pipe(
        finalize(() => {
          this.isLoading = false;
        })
      )
      .subscribe({
        next: game => {
          this.game = game;
        },
        error: () => {
          this.errorMessage = 'Unable to start the game.';
        }
      });
  }

  playCell(position: number): void {
    if (
      !this.game ||
      this.isLoading ||
      this.game.status !== 'InProgress' ||
      this.game.board[position] !== null
    ) {
      return;
    }

    const player: Player =
      this.game.mode === 'Computer'
        ? 'X'
        : this.game.currentPlayer;

    this.errorMessage = '';
    this.isLoading = true;

    this.gameApi
      .makeMove(
        this.game.gameId,
        player,
        position
      )
      .pipe(
        finalize(() => {
          this.isLoading = false;
        })
      )
      .subscribe({
        next: game => {
          this.game = game;

          if (game.status !== 'InProgress') {
            this.loadScoreboard();
          }
        },
        error: error => {
          this.errorMessage =
            error?.error?.detail ??
            'Unable to make the move.';
        }
      });
  }

  private loadScoreboard(): void {
    this.gameApi
      .getScoreboard()
      .subscribe({
        next: scoreboard => {
          this.scoreboard = scoreboard;
        },
        error: () => {
          this.errorMessage =
            'Unable to load scoreboard.';
        }
      });
  }

  undo(): void {
  if (!this.game || this.isLoading) {
    return;
  }

  this.errorMessage = '';
  this.isLoading = true;

  this.gameApi
    .undo(this.game.gameId)
    .pipe(
      finalize(() => {
        this.isLoading = false;
      })
    )
    .subscribe({
      next: game => {
        this.game = game;
      },
      error: error => {
        this.errorMessage =
          error?.error?.detail ??
          'Unable to undo the last move.';
      }
    });
}

resetGame(): void {
  if (!this.game || this.isLoading) {
    return;
  }

  this.errorMessage = '';
  this.isLoading = true;

  this.gameApi
    .resetGame(this.game.gameId)
    .pipe(
      finalize(() => {
        this.isLoading = false;
      })
    )
    .subscribe({
      next: game => {
        this.game = game;
      },
      error: error => {
        this.errorMessage =
          error?.error?.detail ??
          'Unable to reset the game.';
      }
    });
}

resetScoreboard(): void {
  if (this.isLoading) {
    return;
  }

  this.errorMessage = '';
  this.isLoading = true;

  this.gameApi
    .resetScoreboard()
    .pipe(
      finalize(() => {
        this.isLoading = false;
      })
    )
    .subscribe({
      next: scoreboard => {
        this.scoreboard = scoreboard;
      },
      error: error => {
        this.errorMessage =
          error?.error?.detail ??
          'Unable to reset the scoreboard.';
      }
    });
}
}