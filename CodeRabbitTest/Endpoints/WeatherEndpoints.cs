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
			var weatherItem = new WeatherItem(
				GetNextId(),
				request.Date,
				request.TemperatureC,
				request.Summary);

			lock (SyncRoot)
			{
				WeatherItems.Add(weatherItem);
			}

			return Results.Created($"/weathers/{weatherItem.Id}", weatherItem);
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
}

public sealed record WeatherItem(int Id, DateOnly Date, int TemperatureC, string Summary);

public sealed record CreateWeatherRequest(DateOnly Date, int TemperatureC, string Summary);