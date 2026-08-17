# Tic Tac Toe – ABB Software Development Manager Assessment

A browser-based Tic Tac Toe application implemented using Angular and ASP.NET Core.

The application supports:

- Two-player mode
- Player vs Computer mode
- Server-side game state
- Win and draw detection
- Winning-cell highlighting
- Move history
- Undo
- Game reset
- Session scoreboard
- Scoreboard reset
- REST APIs
- Automated unit tests

The backend is the authoritative source of truth for game state and rules.

---

## Technology Stack

### Frontend

- Angular 19
- TypeScript
- Angular HttpClient
- Standalone Angular components
- Responsive CSS

### Backend

- ASP.NET Core Web API
- .NET
- C#
- In-memory repositories
- Native ASP.NET Core OpenAPI
- ProblemDetails-based centralized error handling

### Testing

- xUnit
- Domain unit tests
- Application service tests

---

# Architecture

The solution uses a lightweight layered architecture.

```text
Angular Frontend
       |
       | REST / JSON
       v
ASP.NET Core API
       |
       v
Application Layer
       |
       v
Domain Layer
       ^
       |
Infrastructure