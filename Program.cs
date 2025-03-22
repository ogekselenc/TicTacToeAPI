using Microsoft.EntityFrameworkCore;
using TicTacToeAPI.Data;
using TicTacToeAPI.Repositories;
using TicTacToeAPI.Services;
using TicTacToeAPI.Hubs;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Dodaj konekcioni string iz appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.WebHost.UseUrls("http://127.0.0.1:5000");

// Konfiguracija DbContext-a
builder.Services.AddDbContext<TicTacToeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// Registracija Repository Pattern-a i Unit of Work-a
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IMoveRepository, MoveRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// Registracija servisnog sloja
builder.Services.AddScoped<GameService>();
builder.Services.AddScoped<PlayerService>();
builder.Services.AddScoped<MoveService>();
builder.Services.AddSignalR();



// Dodavanje kontrolera
builder.Services.AddControllers();

// Omogućavanje CORS-a ako frontend zahteva (možeš kasnije podesiti)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<GameHub>("/gameHub");
});

app.UseCors("AllowAll");
app.MapControllers();

app.Run();
