using Microsoft.EntityFrameworkCore;
using TicTacToeAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add database context
builder.Services.AddDbContext<TicTacToeDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.WebHost.UseUrls("http://127.0.0.1:5000", "https://127.0.0.1:5001");


builder.Services.AddControllers();

var app = builder.Build();

//app.UseHttpsRedirection();
//app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.Run();
