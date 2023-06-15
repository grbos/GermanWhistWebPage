using GermanWhistWebPage.Models;
using GermanWhistWebPage.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


var folder = Environment.SpecialFolder.LocalApplicationData;
var path = Environment.GetFolderPath(folder);
String SqliteDbPath = Path.Join("GermanWhist.db");


// Change this to change database 
builder.Services.AddDbContext<GameContext>(opt =>
    {
        opt.UseSqlite($"Data Source={SqliteDbPath}");
    });

builder.Services.AddScoped<CardService>();
builder.Services.AddScoped<PlayerService>();
builder.Services.AddScoped<GameService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
