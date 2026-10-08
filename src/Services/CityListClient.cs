namespace WeatherReport.Services;

/// <summary>
/// Клиент для загрузки списка городов.
/// </summary>
/// <param name="httpClient">HTTP-клиент.</param>
/// <param name="apiUrl">Адрес файла со списком городов.</param>
public sealed class CityListClient(HttpClient httpClient, string apiUrl)
{
  /// <summary>
  /// Получает список городов и убирает дубликаты без учета регистра.
  /// </summary>
  /// <returns>Список городов.</returns>
  public async Task<List<string>> GetUniqueCitiesAsync()
  {
    var content = await httpClient.GetStringAsync(apiUrl);

    return content
      .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
      .Distinct(StringComparer.OrdinalIgnoreCase)
      .ToList();
  }
}
