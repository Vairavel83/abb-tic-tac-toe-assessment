import { Component, inject } from '@angular/core';

import { GameApiService } from './core/services/game-api.service';
import { GameState } from './core/models/game.models';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  private readonly gameApi = inject(GameApiService);

  game: GameState | null = null;
  errorMessage = '';

  createTestGame(): void {
    this.errorMessage = '';

    this.gameApi.createGame('TwoPlayer')
      .subscribe({
        next: game => {
          this.game = game;
        },
        error: error => {
          console.error(error);
          this.errorMessage = 'Unable to create game.';
        }
      });
  }
}