using GermanWhistWebPage.Models;
using GermanWhistWebPage.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//string sqliteDbPath = builder.Configuration.GetConnectionString("SQLiteConnection");

// Change this to change database 
builder.Services.AddDbContext<GameContext>(opt =>
    {
        //opt.UseSqlite(sqliteDbPath);
        opt.UseInMemoryDatabase("germanWhistDB");
    });

builder.Services.AddScoped<CardService>();
builder.Services.AddScoped<BotService>();
builder.Services.AddScoped<GameService>();

builder.Services.AddScoped<JwtService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();


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

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            )
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "GermanWhistOrigin",
                      policy =>
                      {
                          policy.WithOrigins("https://blue-bay-0100c3b03.3.azurestaticapps.net", "http://localhost:4200").AllowAnyMethod().AllowAnyHeader();

                      });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();

    app.UseDeveloperExceptionPage();
    //using (var scope = app.Services.CreateScope())
    //{
    //    var services = scope.ServiceProvider;
    //    try
    //    {
    //        var context = services.GetRequiredService<GameContext>();
    //        context.Database.Migrate();
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}
}


app.UseCors("GermanWhistOrigin");

app.UseHttpsRedirection();


// This Clear() call solves a Bug in mapping of JWT Token to the Dot Net JWT handler
// where the sub claim wrongly gets mapped to the nameIdentifier claim
// https://github.com/IdentityServer/IdentityServer4/issues/2968#issuecomment-510996164
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
