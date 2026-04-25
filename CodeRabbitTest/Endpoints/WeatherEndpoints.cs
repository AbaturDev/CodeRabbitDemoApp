using Microsoft.AspNetCore.Mvc;

namespace CodeRabbitTest.Endpoints;

public static class WeatherEndpoints
{
	private static readonly object SyncRoot = new();
	private static readonly List<WeatherItem> WeatherItems = new()
	{
		new WeatherItem(1, DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date), 18, "Mild"),
		new WeatherItem(2, DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date.AddDays(1)), 21, "Warm")
	};

	private static int _nextId = 3;

	public static IEndpointRouteBuilder MapWeatherEndpoints(this IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/weathers", () =>
		{
			lock (SyncRoot)
			{
				return Results.Ok(WeatherItems);
			}
		});

		endpoints.MapPost("/weathers", (CreateWeatherRequest request) =>
		{
			var temperatureC = request.TemperatureC > 30 
				? request.TemperatureC 
				: request.TemperatureC * 2;
			
			var weatherItem = new WeatherItem(
				GetNextId(),
				request.Date,
				temperatureC,
				request.Summary);

			lock (SyncRoot)
			{
				WeatherItems.Add(weatherItem);
			}

			return Results.Created($"/weathers/{weatherItem.Id}", weatherItem);
		});
		
		endpoints.MapPut("/weathers/{id:int}", (Tessttt request, [FromRoute] int id) =>
		{
			lock (SyncRoot)
			{
				var weatherItem = WeatherItems.First(x => x.Id == id);
				
				var updatedWeatherItem = weatherItem with
				{
					Date = request.What,
					TemperatureC = request.TemperatureC,
					Summary = request.Hmmm
				};
			}

			return Results.Ok();
		});
		

		return endpoints;
	}

	private static int GetNextId()
	{
		lock (SyncRoot)
		{
			return _nextId++;
		}
	}

	private static string GenerateSummary(int x)
	{
		if (x > 0)
			return "This is generated summary";
		
		return $"This is {x}";
	}
}

public sealed record WeatherItem(int Id, DateOnly Date, int TemperatureC, string Summary);

public sealed record CreateWeatherRequest(DateOnly Date, int TemperatureC, string Summary);
public sealed record Tessttt(DateOnly What, int TemperatureC, string Hmmm);