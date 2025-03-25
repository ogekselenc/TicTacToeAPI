using Microsoft.EntityFrameworkCore;
using TicTacToeAPI.Models;
using TicTacToeAPI.Repositories;
using TicTacToeAPI.Interfaces;
using TicTacToeAPI.Services;
using TicTacToeAPI.Mappings;
using TicTacToeAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure DbContext with connection string from configuration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add controllers
builder.Services.AddControllers();

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Register services
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IGameService, GameService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());
});

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "TicTacToe API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TicTacToe API v1"));
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
// Add exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();