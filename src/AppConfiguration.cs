using System.Globalization;
using System.Text.Json;

namespace WeatherReport.Configuration;

/// <summary>
/// Класс для работы с конфигурацией приложения.
/// </summary>
public static class AppConfiguration
{
  /// <summary>
  /// Загруженные настройки.
  /// </summary>
  private static readonly Lazy<Dictionary<string, string>> values = new(Load);

  /// <summary>
  /// Возвращает значение настройки, приведённое к типу <typeparamref name="T"/>.
  /// </summary>
  /// <typeparam name="T">Тип значения.</typeparam>
  /// <param name="key">Ключ настройки.</param>
  /// <returns>Значение настройки.</returns>
  /// <remarks>В качестве <typeparamref name="T"/> могут быть только <see cref="string"/> и <see cref="double"/>.</remarks>
  /// <exception cref="InvalidOperationException">Настройка не задана.</exception>
  /// <exception cref="NotImplementedException">Не реализован обработчик указанного типа.</exception>
  public static T Get<T>(string key)
  {
    if (!values.Value.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
    {
      throw new InvalidOperationException($"Setting '{key}' is not configured.");
    }

    return typeof(T) switch
    {
      var t when t == typeof(string) => (T)(object)value,
      var t when t == typeof(double) => (T)(object)double.Parse(value, CultureInfo.InvariantCulture),
      _ => throw new NotImplementedException($"Type {typeof(T).Name} is not supported.")
    };
  }

  /// <summary>
  /// Загружает настройки.
  /// Debug: из appsettings.json.
  /// Release: из переменных окружения, предварительно дополненных файлом .env.
  /// </summary>
  /// <returns>Словарь настроек.</returns>
  private static Dictionary<string, string> Load()
  {
#if DEBUG
    var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
    return JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(path))
      ?? new Dictionary<string, string>();
#else
    LoadDotEnvFile(Path.Combine(AppContext.BaseDirectory, ".env"));

    return new Dictionary<string, string>
    {
      [Constants.CityListUrlConfigKey] = Environment.GetEnvironmentVariable("CITY_LIST_URL") ?? string.Empty,
      [Constants.WeatherUrlConfigKey] = Environment.GetEnvironmentVariable("WEATHER_URL_TEMPLATE") ?? string.Empty,
      [Constants.HttpClientTimeoutConfigKey] = Environment.GetEnvironmentVariable("HTTP_CLIENT_TIMEOUT_S") ?? string.Empty
    };
#endif
  }

#if !DEBUG
  /// <summary>
  /// Загружает пары из файла .env в переменные окружения.
  /// </summary>
  /// <param name="path">Путь к файлу .env.</param>
  private static void LoadDotEnvFile(string path)
  {
    if (!File.Exists(path))
    {
      Console.Error.WriteLine($".env not found at '{path}', using environment variables only.");
      return;
    }

    foreach (var line in File.ReadLines(path))
    {
      var trimmed = line.Trim();
      if (trimmed.Length == 0 || trimmed.StartsWith('#'))
      {
        continue;
      }

      var separatorIndex = trimmed.IndexOf('=');
      if (separatorIndex <= 0)
      {
        continue;
      }

      var name = trimmed[..separatorIndex].Trim();
      var value = trimmed[(separatorIndex + 1)..].Trim().Trim('"');

      // Настоящие переменные окружения приоритетнее значений из файла.
      if (Environment.GetEnvironmentVariable(name) is null)
      {
        Environment.SetEnvironmentVariable(name, value);
      }
    }
  }
#endif
}
