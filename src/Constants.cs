namespace WeatherReport;

/// <summary>
/// Общие константы приложения.
/// </summary>
internal static class Constants
{
  /// <summary>
  /// Текст для значений, которые не удалось получить.
  /// </summary>
  public const string UnspecifiedData = "unspecified";

  /// <summary>
  /// Ключ настройки: адрес документа со списком городов.
  /// </summary>
  public const string CityListUrlConfigKey = "CityListUrl";

  /// <summary>
  /// Ключ настройки: шаблон адреса API погоды.
  /// </summary>
  public const string WeatherUrlConfigKey = "WeatherUrlTemplate";

  /// <summary>
  /// Ключ настройки: таймаут HTTP-клиента в секундах.
  /// </summary>
  public const string HttpClientTimeoutConfigKey = "HttpClientTimeoutS";
}
