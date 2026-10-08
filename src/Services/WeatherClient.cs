using System.Text.Json;
using System.Globalization;
using WeatherReport.Types;

namespace WeatherReport.Services;

/// <summary>
/// Клиент API погоды в городе.
/// </summary>
/// <param name="httpClient">HTTP-клиент.</param>
/// <param name="apiUrlTemplate">Шаблон адреса, где {0} заменяется на город.</param>
public sealed class WeatherClient(HttpClient httpClient, string apiUrlTemplate)
{
  /// <summary>
  /// Получает текущую температуру и страну для города.
  /// </summary>
  /// <param name="city">Название города.</param>
  /// <returns>Сущность погоды в городе.</returns>
  public async Task<WeatherData> GetWeatherAsync(string city)
  {
    var url = string.Format(apiUrlTemplate, Uri.EscapeDataString(city));

    await using var stream = await httpClient.GetStreamAsync(url);
    using var json = await JsonDocument.ParseAsync(stream);

    var temperature = json.RootElement
      .GetProperty("current_condition")[0]
      .GetProperty("temp_C")
      .GetString();

    var country = json.RootElement
      .GetProperty("nearest_area")[0]
      .GetProperty("country")[0]
      .GetProperty("value")
      .GetString();

    return new WeatherData(
      city,
      int.TryParse(temperature, CultureInfo.InvariantCulture, out var currentTemperature) ? currentTemperature : null,
      string.IsNullOrEmpty(country) ? Constants.UnspecifiedData : country);
  }
}
