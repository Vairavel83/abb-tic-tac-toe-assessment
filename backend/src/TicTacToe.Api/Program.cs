using System.Text.Json.Serialization;
using TicTacToe.Application.Interfaces;
using TicTacToe.Application.Services;
using TicTacToe.Domain.Services;
using TicTacToe.Infrastructure.Repositories;
using TicTacToe.Api.ExceptionHandling;

var builder = WebApplication.CreateBuilder(args);

// Controllers + JSON configuration
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
// Native ASP.NET Core OpenAPI
builder.Services.AddOpenApi();

// Domain services
builder.Services.AddSingleton<
    IComputerMoveStrategy,
    BasicComputerMoveStrategy>();

// Infrastructure repositories
builder.Services.AddSingleton<
    IGameRepository,
    InMemoryGameRepository>();

builder.Services.AddSingleton<
    IScoreboardRepository,
    InMemoryScoreboardRepository>();

// Application services
builder.Services.AddScoped<GameService>();
builder.Services.AddScoped<ScoreboardService>();

var app = builder.Build();
app.UseExceptionHandler();
// Expose OpenAPI only during development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

// Required later for API integration tests
public partial class Program
{
}