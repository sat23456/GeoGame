using dotenv.net;
using GeoGame.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MongoDbService>();
DotEnv.Load();
builder.Configuration.AddEnvironmentVariables();
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

