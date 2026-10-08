using System.Globalization;
using System.Text;
using WeatherReport;
using WeatherReport.Configuration;
using WeatherReport.Services;
using WeatherReport.Types;

Console.OutputEncoding = Encoding.UTF8;

using var httpClient = new HttpClient
{
  Timeout = TimeSpan.FromSeconds(AppConfiguration.Get<double>(Constants.HttpClientTimeoutConfigKey))
};

var cityLoader = new CityListClient(httpClient, AppConfiguration.Get<string>(Constants.CityListUrlConfigKey));
var weatherClient = new WeatherClient(httpClient, AppConfiguration.Get<string>(Constants.WeatherUrlConfigKey));

var cities = new List<string>();
try
{
  cities = await cityLoader.GetUniqueCitiesAsync();
}
catch (Exception ex)
{
  Console.Error.WriteLine($"An error occurred when getting cities: {ex.Message}");
  return;
}

var weatherTasks = cities.Select(async city =>
{
  try
  {
    return await weatherClient.GetWeatherAsync(city);
  }
  catch (Exception ex)
  {
    Console.Error.WriteLine($"Skipped '{city}': {ex.Message}");
    return null;
  }
});

var weatherData = (await Task.WhenAll(weatherTasks)).OfType<WeatherData>().ToList();

foreach (var item in weatherData.OrderBy(x => x.Country).ThenBy(x => x.City))
{
  Console.WriteLine($"{item.City}, {item.Country} {FormatTemperature(item.TemperatureCelsius)}");
}

Console.WriteLine();

var statistics = weatherData
  .GroupBy(item => item.Country)
  .Select(group =>
  {
    var temperatures = group
      .Where(x => x.TemperatureCelsius.HasValue)
      .Select(x => x.TemperatureCelsius!.Value)
      .ToList();

    return temperatures.Count == 0
      ? new CountryStatistics(group.Key, group.Count(), null, null, null)
      : new CountryStatistics(group.Key, group.Count(), temperatures.Average(), temperatures.Min(), temperatures.Max());
  })
  .OrderBy(item => item.Country);

foreach (var item in statistics)
{
  Console.WriteLine(
    $"{item.Country} — {item.CityCount} cities, " +
    $"avg: {FormatTemperature(item.AverageTemperatureCelsius)}, " +
    $"min: {FormatTemperature(item.MinTemperatureCelsius)}, " +
    $"max: {FormatTemperature(item.MaxTemperatureCelsius)}");
}

static string FormatTemperature(double? celsius)
{
  return celsius.HasValue
    ? $"{Math.Round(celsius.Value, MidpointRounding.AwayFromZero).ToString("+0;-0;0", CultureInfo.InvariantCulture)} °C"
    : Constants.UnspecifiedData;
}
