# Architecture Notes

This is not intended to be a generic "enterprise architecture" for Tic Tac Toe. My goal was to keep the assessment easy to run and review while still keeping the important responsibilities separated.

## High-level view

```text
Angular UI
    |
    | REST / JSON
    v
ASP.NET Core API
    |
    v
Application
    |
    v
Domain
    ^
    |
Infrastructure
```

The dependency direction is deliberate: the game rules do not depend on ASP.NET Core, Angular, or the in-memory storage implementation.

## Backend projects

### TicTacToe.Domain

This is where the core game behavior lives.

Main responsibilities:

- board state
- move validation
- current player
- move history
- win/draw evaluation
- reset
- single-move undo
- computer move strategy
- scoreboard model

The `Game` object is responsible for keeping a game in a valid state.

I kept the board as nine indexed cells (`0..8`). It makes API communication and win combinations simple, while row/column values can be calculated when presenting move history.

### TicTacToe.Application

This layer coordinates use cases that involve more than one domain operation.

A good example is Computer mode.

```text
Human X request
      |
      v
Game.MakeMove(X)
      |
      +---- game completed? ---- yes ---> return
      |
      no
      v
IComputerMoveStrategy.SelectMove
      |
      v
Game.MakeMove(O)
      |
      v
return latest state
```

The computer pair-undo behavior also belongs here. The domain knows how to undo one move; the application workflow decides that Computer mode requires two undos.

This separation kept the `Game` object from knowing about HTTP requests or UI workflows.

### TicTacToe.Infrastructure

The assessment allows in-memory state, so I implemented:

- `InMemoryGameRepository`
- `InMemoryScoreboardRepository`

The application depends on repository interfaces rather than the concrete classes.

That means a future SQLite implementation could replace the in-memory repository without moving persistence code into the domain.

The repositories are singleton services because a new in-memory repository per HTTP request would lose the previously created game.

### TicTacToe.Api

The API layer is deliberately thin.

It handles:

- routes
- request/response contracts
- mapping domain state to API responses
- dependency injection
- CORS for the Angular development server
- OpenAPI
- translation of exceptions to HTTP responses

I avoided returning the domain models directly. The API has explicit response contracts so the HTTP shape is not accidentally coupled to internal domain implementation.

## Frontend

The Angular frontend has three simple concerns:

```text
Component
   |
   v
GameApiService
   |
   v
.NET API
```

The component manages UI state such as loading/error messages and renders the returned `GameState`.

`GameApiService` owns the HTTP calls.

TypeScript models mirror the API contract.

The frontend may prevent an obviously invalid click, such as clicking an occupied cell, but that is only a UI convenience. The backend still validates every move.

## Backend state ownership

I treated the backend as the authoritative game state.

For example, Angular does not do this:

```text
click cell
→ update local board
→ calculate winner locally
→ later tell backend
```

It does this instead:

```text
click cell
→ POST move
→ backend validates and updates state
→ Angular renders returned state
```

This was one of the most important choices in the implementation because it prevents the frontend and backend from drifting into different interpretations of the game.

## Computer strategy

`IComputerMoveStrategy` separates "how the computer chooses a cell" from the rest of the game workflow.

The implementation follows:

```text
Can O win now?
    yes → win
    no
Can X win next?
    yes → block
    no
Center free?
    yes → center
    no
Corner free?
    yes → corner
    no
First available cell
```

Board cloning is used while evaluating candidate moves so the real board is not mutated during strategy calculation.

## Undo

I intentionally kept one basic undo operation in the domain:

```text
UndoLastMove()
```

Two Player mode calls it once.

Computer mode calls it twice at the application layer:

```text
remove computer O
remove previous human X
return to X
```

For completed games I chose the assessment's Option A: Undo is disabled once the game is Won or Drawn.

## Scoreboard

The scoreboard is session-level state, separate from an individual board reset.

When a game reaches a final state, the application records:

- X win
- O win
- draw

A game also tracks whether its result has already been recorded. This prevents the same completed round from incrementing the scoreboard more than once.

`ResetGame` clears the round but not the scoreboard.

`ResetScoreboard` clears only the session totals.

## Error handling

Game rules throw normal domain/application exceptions.

The controller does not repeat try/catch logic around every action. A centralized exception handler maps expected exceptions to HTTP responses.

Examples:

```text
KeyNotFoundException        → 404
ArgumentOutOfRangeException → 400
InvalidOperationException   → 400
unexpected exception        → 500
```

For unexpected errors the client gets a generic response instead of an internal stack trace.

## Things I deliberately did not add

I considered the size of the problem before choosing patterns.

I did not add:

- CQRS
- MediatR
- Entity Framework
- a database
- AutoMapper
- a separate microservice
- authentication
- a state management library in Angular

Those would make the solution bigger without helping the assessment requirements.

The layering is there to make responsibilities clear, not to demonstrate the maximum number of patterns.

## What I would change for a production version

The first changes I would make are persistence, integration tests, environment-based configuration, and observability.

If the application needed real multiplayer, I would also introduce a concurrency strategy for game updates and probably SignalR/WebSockets rather than polling REST state.
