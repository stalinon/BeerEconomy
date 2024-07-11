namespace BeerEconomy.Common.Helpers;

/// <summary>
///     Флаги для выбора режима разбора и форматирования строки запроса
/// </summary>
[Flags]
public enum QueryStringOptions
{
    /// <summary>
    ///     Значение по умолчанию
    /// </summary>
    None = 0,

    /// <summary>
    ///     Форматировать массивы как "?foo=x,y" вместо "?foo=x&quot;foo=y"
    /// </summary>
    Csv = 1,
}