using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace BeerEconomy.Common.Helpers;

/// <summary>
///     Объект, который можно преобразовать в query-строку
/// </summary>
public interface IQueryStringSource
{
    /// <summary>
    ///     Преобразовать в query-строку
    /// </summary>
    IReadOnlyDictionary<string, StringValues> ToQueryString(JsonSerializerSettings serializerSettings = null);
}