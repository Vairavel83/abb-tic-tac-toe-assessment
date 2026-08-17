# Tic Tac Toe Assessment

This repository contains my implementation of the Tic Tac Toe assessment using an Angular frontend and an ASP.NET Core Web API backend.

I kept the solution intentionally small, but I still separated the game rules, application workflow, persistence, and API concerns so that the code is easy to explain and change during review.

## What is implemented

- Two Player mode
- Player vs Computer mode
- 3 × 3 game board
- Turn validation
- Row, column, and diagonal win detection
- Draw detection
- Winning-cell highlight
- Move history with move number, player, row, and column
- Undo
  - Two Player mode: removes the latest move
  - Computer mode: removes the computer move and the previous human move together
- Reset Game
- Session scoreboard for X wins, O wins, and draws
- Reset Scoreboard
- REST API between Angular and .NET
- Centralized API error handling
- Backend unit tests

## Tech stack

### Frontend
- Angular 20
- TypeScript
- Angular HttpClient
- Standalone components
- Responsive CSS

### Backend
- .NET 10
- ASP.NET Core Web API
- C#
- REST/JSON
- In-memory repositories
- Native ASP.NET Core OpenAPI
- xUnit

The frontend was tested with Node.js 22.14.0.

## Repository structure

```text
.
├── backend
│   ├── src
│   │   ├── TicTacToe.Domain
│   │   ├── TicTacToe.Application
│   │   ├── TicTacToe.Infrastructure
│   │   └── TicTacToe.Api
│   └── tests
│       ├── TicTacToe.Domain.Tests
│       └── TicTacToe.Application.Tests
├── frontend
└── docs
    ├── ARCHITECTURE.md
    └── AI_USAGE.md
```

## Running the backend

From the repository root:

```powershell
cd backend
dotnet restore
dotnet build
dotnet test
dotnet run --project .\src\TicTacToe.Api\TicTacToe.Api.csproj
```

The API is configured to run at:

```text
http://localhost:5165
```

I verified the backend from a fresh GitHub ZIP download. The current test suite contains 29 passing tests.

If port `5165` is already in use, stop the existing process before starting another API instance.

## Running the frontend

Open a second terminal:

```powershell
cd frontend
npm ci
npm run build
npm start
```

Then open:

```text
http://localhost:4200
```

The backend must be running at `http://localhost:5165`.

I used `npm ci` for the clean-run verification so the frontend is installed from the committed lock file rather than from an existing local `node_modules` folder.

## API summary

| Method | Endpoint | Purpose |
|---|---|---|
| POST | `/api/games` | Create a game |
| GET | `/api/games/{id}` | Get current game state |
| POST | `/api/games/{id}/moves` | Submit a move |
| POST | `/api/games/{id}/undo` | Undo the latest move / move pair |
| POST | `/api/games/{id}/reset` | Reset the current game |
| GET | `/api/scoreboard` | Get session scoreboard |
| POST | `/api/scoreboard/reset` | Reset session scoreboard |

Example create-game request:

```json
{
  "mode": "Computer"
}
```

Example move request:

```json
{
  "player": "X",
  "position": 0
}
```

Board positions use zero-based indexes internally:

```text
0 | 1 | 2
---------
3 | 4 | 5
---------
6 | 7 | 8
```

The response also exposes row and column values in the move history for easier display in the UI.

## Important design choices

### Backend is the source of truth

The frontend does not calculate winners, turns, computer moves, or scoreboard values. It sends commands to the API and renders the latest game state returned by the backend.

This avoids having two independent copies of the game rules.

### Computer move strategy

The computer always plays as O and uses the required priority:

1. Win if possible
2. Block X if X can win next
3. Take the center
4. Take an available corner
5. Take any remaining valid cell

The strategy is deterministic, which also makes the behavior straightforward to test.

### Undo after game completion

I chose **Option A: Undo is disabled after a Won or Draw state**.

Once a result is recorded in the scoreboard, that round is treated as complete. This keeps scoreboard behavior predictable and avoids compensating a completed score after an undo.

### Persistence

Game state and scoreboard state are stored in memory. The repository instances are registered as singletons so state is retained across HTTP requests while the backend is running.

State is intentionally lost when the API process restarts.

## Tests

Run all backend tests with:

```powershell
cd backend
dotnet test
```

The tests cover the main game rules and application workflows, including:

- valid and invalid moves
- turn switching
- occupied cells
- row win
- column win
- diagonal win
- draw
- move after completion
- reset
- Two Player undo
- Computer mode undo
- computer winning move
- computer block
- center/corner/fallback selection
- game creation and retrieval
- computer-mode orchestration
- scoreboard update
- score recorded once
- scoreboard reset

At the final clean-run check: **29 tests passed, 0 failed**.

## Error handling

Domain/application errors are translated at the API boundary.

Examples:

- unknown game → `404 Not Found`
- invalid position → `400 Bad Request`
- wrong player / occupied cell / move after completion → `400 Bad Request`

Example:

```json
{
  "title": "Invalid operation",
  "status": 400,
  "detail": "It is X's turn."
}
```

## Assumptions and clarifications

- X always starts a new round.
- In Computer mode, the human is X and the computer is O.
- The same game ID is retained when Reset Game is used.
- Reset Game clears the board, move history, winner/draw state, and restores X as the current player.
- Reset Game does not reset the scoreboard.
- Reset Scoreboard does not reset the current game.
- Undo is disabled after game completion.
- In-memory state is sufficient for this assessment.
- A mode change starts a new game rather than changing the mode of an existing game.

## Known limitations

- Game and scoreboard state do not survive an API restart.
- There is no authentication or user account concept.
- The solution does not synchronize one game across multiple browsers in real time.
- The computer player follows a simple deterministic strategy rather than minimax.
- The API URL is configured for local development.
- Frontend component tests were not added; the automated coverage is focused on backend game rules and application behavior.

## Future improvements

If this were taken beyond the assessment, I would consider:

- SQLite or another persistent store
- API integration tests
- Angular component tests
- minimax / difficulty levels for the computer player
- SignalR for real-time multiplayer
- environment-based frontend/API configuration
- Docker support
- CI pipeline
- structured logging and telemetry

## Architecture notes

More detail on the layering and the main design decisions is available in [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).
