namespace WeatherReport.Types;

/// <summary>
/// Сводная статистика температуры по стране.
/// </summary>
/// <param name="Country">Название страны.</param>
/// <param name="CityCount">Количество городов в стране.</param>
/// <param name="AverageTemperatureCelsius">Средняя температура, °C; null, если данных нет.</param>
/// <param name="MinTemperatureCelsius">Минимальная температура, °C; null, если данных нет.</param>
/// <param name="MaxTemperatureCelsius">Максимальная температура, °C; null, если данных нет.</param>
public sealed record CountryStatistics(
  string Country,
  int CityCount,
  double? AverageTemperatureCelsius,
  int? MinTemperatureCelsius,
  int? MaxTemperatureCelsius);

/// <summary>
/// Текущая погода в городе.
/// </summary>
/// <param name="City">Название города.</param>
/// <param name="TemperatureCelsius">Текущая температура, °C; null, если данных нет.</param>
/// <param name="Country">Название страны.</param>
public sealed record WeatherData(string City, int? TemperatureCelsius, string Country);
