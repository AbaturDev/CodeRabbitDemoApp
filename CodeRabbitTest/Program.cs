using CodeRabbitTest.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapWeatherEndpoints();

if (app.Environment.IsDevelopment())
    throw new Exception("Test");

app.Run();
