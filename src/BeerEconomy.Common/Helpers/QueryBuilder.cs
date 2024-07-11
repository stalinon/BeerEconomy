using System.Collections;
using System.Collections.Immutable;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Scraper.Common.Models;

namespace BeerEconomy.Common.Helpers;

/// <summary>
///     Построитель для query-строк
/// </summary>
public sealed class QueryBuilder : IQueryStringSource, IEnumerable<KeyValuePair<string, StringValues>>
{
    #region fields

    /// <summary>
    ///     Значение по умолчанию для <see cref="QueryStringOptions"/>
    /// </summary>
    internal const QueryStringOptions DefaultOptions = QueryStringOptions.None;

    private readonly QueryStringOptions _options;
    private readonly Dictionary<string, StringValues> _queryString = new();
    private readonly JsonSerializerSettings _serializerSettings;

    #endregion

    #region .ctor

    private QueryBuilder(QueryStringOptions options, JsonSerializerSettings serializerSettings = null)
    {
        _options = options;
        _serializerSettings = serializerSettings;
    }

    #endregion

    #region factory methods

    /// <summary>
    ///     Создать новый построитель
    /// </summary>
        
    public static QueryBuilder New(QueryStringOptions? options = null, JsonSerializerSettings serializerSettings = null)
        => new(options ?? DefaultOptions, serializerSettings);

    /// <summary>
    ///     Создать новый построитель из параметров запроса
    /// </summary>
        
    public static QueryBuilder FromQuery(
        object query,
        QueryStringOptions? options = null,
        JsonSerializerSettings serializerSettings = null)
        => QueryBuilderGenerator.Generate(query, options, serializerSettings);

    /// <summary>
    ///     Создать новый построитель из query-строки
    /// </summary>
        
    public static QueryBuilder Parse(
        string query,
        QueryStringOptions? options = null,
        JsonSerializerSettings serializerSettings = null)
    {
        var builder = new QueryBuilder(options ?? DefaultOptions, serializerSettings);

        var dict = QueryHelpers.ParseNullableQuery(query ?? "");
        builder.CombineWith(dict);

        return builder;
    }

    #endregion

    #region public methods

    /// <summary>
    ///     Сколько параметров задано
    /// </summary>
    public int Count => _queryString.Count;

    /// <summary>
    ///     Объединить два построителя
    /// </summary>
        
    public QueryBuilder CombineWith(IQueryStringSource source)
    {
        if (source != null)
        {
            CombineWith(source.ToQueryString(_serializerSettings));
        }

        return this;
    }

    /// <summary>
    ///     Объединить два постоителя
    /// </summary>
        
    public QueryBuilder CombineWith(IReadOnlyDictionary<string, StringValues> source)
    {
        if (source != null)
        {
            foreach (var (key, values) in source)
            {
                _queryString.TryGetValue(key, out var combinedValues);

                if (_options.HasFlag(QueryStringOptions.Csv))
                {
                    combinedValues = new StringValues(
                        combinedValues.Concat(SplitIntoParts(values)).ToArray()
                    );
                }
                else
                {
                    combinedValues = new StringValues(
                        combinedValues.Concat(values).ToArray()
                    );
                }

                _queryString[key] = combinedValues;
            }
        }

        return this;

        static IEnumerable<string> SplitIntoParts(StringValues values)
        {
            foreach (var value in values)
            {
                if (value != null)
                {
                    foreach (var part in value.Split(','))
                    {
                        yield return part;
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Добавить параметр query-строки
    /// </summary>
        
    public QueryBuilder Add( string key, string value)
    {
        AddString(key, value);
        return this;
    }

    /// <summary>
    ///     Добавить параметр query-строки
    /// </summary>
        
    public QueryBuilder Add<T>( string key, T value)
        where T : IConvertible
    {
        AddGeneric(key, value);
        return this;
    }

    ///
    /// <summary>
    ///     Добавить параметр query-строки
    /// </summary>
        
    public QueryBuilder Add<T>( string key, T? value)
        where T : struct, IConvertible
    {
        AddNullableGeneric(key, value);
        return this;
    }

    /// <summary>
    ///     Добавить параметр query-строки
    /// </summary>
        
    public QueryBuilder Add<T>( string key, T[] values)
        where T : IConvertible
    {
        AddArray(key, values);
        return this;
    }

    /// <summary>
    ///     Удалить параметр query-строки
    /// </summary>
        
    public QueryBuilder Remove( string key)
    {
        _queryString.Remove(key);
        return this;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (_queryString.Count == 0)
        {
            return "";
        }

        return string.Join("&", IterateQueryStringParts());

        IEnumerable<string> IterateQueryStringParts()
        {
            foreach (var (key, values) in _queryString)
            {
                if (_options.HasFlag(QueryStringOptions.Csv))
                {
                    yield return FormatQueryPartCsv(key, values);
                }
                else
                {
                    yield return FormatQueryPartFlat(key, values);
                }
            }
        }

        // преобразование в строку запроса вида: key=value1&key=value2...key=valueN
        static string FormatQueryPartFlat(string key, StringValues values)
        {
            var queriedArray = new List<string>();

            foreach (var value in values)
            {
                queriedArray.Add($"{key}={Uri.EscapeDataString(value)}");
            }

            return string.Join("&", queriedArray);
        }

        // преобразование в строку запроса вида: key=value1,value2...,valueN
        static string FormatQueryPartCsv(string key, StringValues values)
        {
            var value = string.Join(",", from v in values select Uri.EscapeDataString(v));
            return $"{key}={value}";
        }
    }

    /// <summary>
    ///     ToString()
    /// </summary>
    public static explicit operator string(QueryBuilder x)
    {
        return x.ToString();
    }

    #endregion

    #region IEnumerable

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator() => _queryString.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #region IQueryStringSource

    /// <inheritdoc />
    public IReadOnlyDictionary<string, StringValues> ToQueryString(JsonSerializerSettings serializerSettings = null)
    {
        return _queryString;
    }

    #endregion

    #region helper methods

    /// <summary>
    ///     Добавить к запросу параметр типа <see cref="string"/>
    /// </summary>
    internal void AddString( string key, string value)
    {
        AddValue(key, value);
    }

    /// <summary>
    ///     Добавить к запросу параметр типа <typeparamref name="T"/>
    /// </summary>
    internal void AddGeneric<T>( string key, T value)
    {
        if (value == null)
        {
            return;
        }

        var str = QueryBuilderConverter.ConvertComplexObject(value, key, null, _serializerSettings);

        var strValues = str.Split("&").Select(v => v.Split("=")).ToArray();

        if (strValues.Length > 1)
        {
            foreach (var strV in strValues)
            {
                AddString(key + strV[0], strV[1]);
            }

            return;
        }

        AddString(key, str);
    }

    /// <summary>
    ///     Добавить к запросу параметр типа <typeparamref name="T?"/>
    /// </summary>
    internal void AddNullableGeneric<T>( string key, T? value)
        where T : struct
    {
        if (value == null)
        {
            return;
        }

        AddGeneric(key, value.Value);
    }

    /// <summary>
    ///     Добавить к запросу параметр типа <typeparamref name="T[]"/>
    /// </summary>
    internal void AddArray<T>( string key, T[] values)
    {
        if (values == null)
        {
            return;
        }

        if (values.Length > 0)
        {
            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                AddGeneric($"{key}[{i}]", value);
            }
        }
        else
        {
            AddValue(key, "");
        }
    }

    /// <summary>
    ///     Добавить к запросу параметр типа <see cref="IEnumerable{T}"/>
    /// </summary>
    internal void AddEnumerable<T>( string key, IEnumerable<T> values)
    {
        if (values == null)
        {
            return;
        }

        if (values is ImmutableArray<T> array && array.IsDefaultOrEmpty)
        {
            return;
        }

        AddArray(key, values.ToArray());
    }

    private void AddValue(string key, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        if (!_queryString.TryGetValue(key, out var list))
        {
            list = StringValues.Empty;
        }

        list = StringValues.Concat(list, value);
        _queryString[key] = list;
    }

    #endregion
}