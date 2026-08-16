import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  CreateGameRequest,
  GameMode,
  GameState,
  MoveRequest,
  Player
} from '../models/game.models';

import { Scoreboard } from '../models/scoreboard.model';

@Injectable({
  providedIn: 'root'
})
export class GameApiService {
  private readonly http = inject(HttpClient);

  private readonly baseUrl = 'http://localhost:5165/api';

  createGame(mode: GameMode): Observable<GameState> {
    const request: CreateGameRequest = {
      mode
    };

    return this.http.post<GameState>(
      `${this.baseUrl}/games`,
      request
    );
  }

  getGame(gameId: string): Observable<GameState> {
    return this.http.get<GameState>(
      `${this.baseUrl}/games/${gameId}`
    );
  }

  makeMove(
    gameId: string,
    player: Player,
    position: number
  ): Observable<GameState> {
    const request: MoveRequest = {
      player,
      position
    };

    return this.http.post<GameState>(
      `${this.baseUrl}/games/${gameId}/moves`,
      request
    );
  }

  undo(gameId: string): Observable<GameState> {
    return this.http.post<GameState>(
      `${this.baseUrl}/games/${gameId}/undo`,
      {}
    );
  }

  resetGame(gameId: string): Observable<GameState> {
    return this.http.post<GameState>(
      `${this.baseUrl}/games/${gameId}/reset`,
      {}
    );
  }

  getScoreboard(): Observable<Scoreboard> {
    return this.http.get<Scoreboard>(
      `${this.baseUrl}/scoreboard`
    );
  }

  resetScoreboard(): Observable<Scoreboard> {
    return this.http.post<Scoreboard>(
      `${this.baseUrl}/scoreboard/reset`,
      {}
    );
  }
}