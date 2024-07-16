using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace BeerEconomy.Common.Helpers
{
    /// <summary>
    ///     Базовый класс для всех GET-запросов.
    ///     Умеет автоматически преобразовывать себя в словарь параметров и в query-строку
    /// </summary>
    public abstract class QueryBase : IQueryStringSource
    {
        /// <summary>
        ///     Преобразовать в строку в формате GET запроса
        /// </summary>
        public sealed override string ToString()
        {
            var settings = new JsonSerializerSettings();
            var builder = QueryBuilder.FromQuery(this, serializerSettings: settings);
            var str = builder.ToString();
            return str;
        }

        /// <inheritdoc />
        IReadOnlyDictionary<string, StringValues> IQueryStringSource.ToQueryString(JsonSerializerSettings serializerSettings)
        {
            var builder = QueryBuilder.FromQuery(this, null, serializerSettings);
            var queryString = builder.ToQueryString(serializerSettings);
            return queryString;
        }
    }
}
