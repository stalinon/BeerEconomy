namespace BeerEconomy.Common;

/// <summary>
///     Конфигурация
/// </summary>
public static class Configs
{
    /// <summary>
    ///     Строка подключения к БД
    /// </summary>
    public const string CONNECTION_STRING = nameof(CONNECTION_STRING);

    /// <summary>
    ///     Логин админа
    /// </summary>
    public const string ADMIN_USERNAME = nameof(ADMIN_USERNAME);
    
    /// <summary>
    ///     Пароль админа
    /// </summary>
    public const string ADMIN_PASSWORD = nameof(ADMIN_PASSWORD);
    
    /// <summary>
    ///     Название заголовка
    /// </summary>
    public const string API_KEY_HEADER_NAME = "X-API-KEY";
    
    /// <summary>
    ///     Адрес сервиса данных
    /// </summary>
    public const string DATA_SERVICE_URL = nameof(DATA_SERVICE_URL);
    
    /// <summary>
    ///     Адрес коллектора
    /// </summary>
    public const string COLLECTOR_SERVICE_URL = nameof(COLLECTOR_SERVICE_URL);
}