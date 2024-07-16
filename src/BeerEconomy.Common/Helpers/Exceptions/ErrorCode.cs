namespace BeerEconomy.Common.Helpers.Exceptions;

/// <summary>
///     Ошибка
/// </summary>
public enum ErrorCode
{
    /// <summary>
    ///     Неверный формат запроса
    /// </summary>
    BAD_REQUEST = 400,
    
    /// <summary>
    ///     Неавторизован
    /// </summary>
    UNAUTHORIZED = 401,
    
    /// <summary>
    ///     Запрещено
    /// </summary>
    FORBIDDEN = 403,
    
    /// <summary>
    ///     Не найдено
    /// </summary>
    NOT_FOUND = 404,
    
    /// <summary>
    ///     Конфликт
    /// </summary>
    CONFLICT = 409,
    
    /// <summary>
    ///     Внутренняя ошибка сервера
    /// </summary>
    INTERNAL_SERVER_ERROR = 500,
    
    /// <summary>
    ///     Сервис недоступен
    /// </summary>
    SERVICE_UNAVAILABLE = 503
}
