using Microsoft.EntityFrameworkCore;
using TicTacToeAPI.Data;
using TicTacToeAPI.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add database context
builder.Services.AddDbContext<TicTacToeDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.WebHost.UseUrls("http://127.0.0.1:5000", "https://127.0.0.1:5001");

// Add SignalR services
builder.Services.AddSignalR();

// Detailed logging for SignalR
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://your-client-domain")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Map the SignalR hub
app.MapHub<GameHub>("/gameHub").RequireCors("AllowAll");

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.Run();