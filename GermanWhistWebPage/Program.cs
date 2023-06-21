using GermanWhistWebPage.Models;
using GermanWhistWebPage.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

string sqliteDbPath = configuration.GetConnectionString("SQLiteConnection");

// Change this to change database 
builder.Services.AddDbContext<GameContext>(opt =>
    {
        opt.UseSqlite(sqliteDbPath);
    });

builder.Services.AddScoped<CardService>();
builder.Services.AddScoped<PlayerService>();
builder.Services.AddScoped<GameService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddIdentityCore<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<GameContext>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// used for static files in HTTP page
app.UseDefaultFiles();
app.UseStaticFiles();



app.UseAuthorization();

app.MapControllers();

app.Run();
