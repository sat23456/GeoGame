using ConfigurationSubstitution;
using dotenv.net;
using GeoGame.Services;
using static Microsoft.Extensions.Configuration.ConfigurationBuilder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MongoDbService>();
DotEnv.Load();
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.json").EnableSubstitutions("%","%").Build();
// Add services to the container.
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();    
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

