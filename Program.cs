using Microsoft.EntityFrameworkCore;
using TicTacToeAPI.Data;
using TicTacToeAPI.Repositories;



var builder = WebApplication.CreateBuilder(args);

// Dodaj konekcioni string iz appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Konfiguracija DbContext-a
builder.Services.AddDbContext<TicTacToeDbContext>(options =>
    options.UseSqlite(connectionString));

// Registracija Repository Pattern-a i Unit of Work-a
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IMoveRepository, MoveRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// Registracija servisnog sloja
builder.Services.AddScoped<GameService>();
builder.Services.AddScoped<PlayerService>();
builder.Services.AddScoped<MoveService>();


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

// Konfiguracija API-ja
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Podešavanje HTTP Request Pipeline-a
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

app.Run();
