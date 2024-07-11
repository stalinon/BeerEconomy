using System.Collections;
using System.Globalization;
using System.Reflection;
using AutoMapper.Internal;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Scraper.Common.Models;

namespace BeerEconomy.Common.Helpers;

/// <summary>
///     Конвертер для <see cref="QueryBuilder"/>.
///     Преобразует типизированные значения в строки
/// </summary>
internal static class QueryBuilderConverter
{
    /// <summary>
    ///     Конвертер (нетипизированный)
    /// </summary>
    private interface IConvertFunc
    {
        /// <summary>
        ///     Преобразовать значение в строку
        /// </summary>
        string Convert(object value, string format);
    }

    /// <summary>
    ///     Конвертер (типизированный)
    /// </summary>
    private interface IConvertFunc<in T>
    {
        /// <summary>
        ///     Преобразовать значение в строку
        /// </summary>
        string Convert(T value, string format);
    }

    /// <summary>
    ///     Конвертер (реализация)
    /// </summary>
    private sealed class ConvertFunc<T> : IConvertFunc<T>, IConvertFunc
    {
        private readonly Func<T, string, string> _func;

        public ConvertFunc(Func<T, string> func)
            : this((value, format) => func(value))
        {
        }

        public ConvertFunc(Func<T, string, string> func)
        {
            _func = func;
        }

        public string Convert(T value, string format)
        {
            return _func(value, format);
        }

        public string Convert(object value, string format)
        {
            return value is T typedValue ? _func(typedValue, format) : "";
        }
    }

    private static readonly Dictionary<Type, IConvertFunc> Converters
        = new()
        {
            [typeof(bool)] = new ConvertFunc<bool>(x => x ? "true" : "false"),
            [typeof(DateTime)] = new ConvertFunc<DateTime>((x, format) => x.ToString(format ?? "u", CultureInfo.InvariantCulture)),
        };

    /// <summary>
    ///     Преобразовать значение в строку
    /// </summary>
    public static string Convert<T>(
        T value,
        string format = null,
        JsonSerializerSettings serializerSettings = null)
    {
        if (Converters.TryGetValue(typeof(T), out var obj) && obj is IConvertFunc<T> func)
        {
            var str = func.Convert(value, format);
            return str;
        }

        if (typeof(T).IsEnum)
        {
            var metadata = EnumMetadata.Get(typeof(T), serializerSettings);

            if (metadata.TryConvert(value, out var str))
            {
                return str;
            }
        }

        if (serializerSettings != null && serializerSettings.Converters.Any(t => t.CanConvert(typeof(T))))
        {
            var str = JsonConvert.SerializeObject(value, serializerSettings).Replace("\"", string.Empty);
            return str;
        }

        if (value is IFormattable formattable)
        {
            var str = formattable.ToString(format, CultureInfo.InvariantCulture);
            return str;
        }

        if (value is IConvertible convertible)
        {
            var str = convertible.ToString(CultureInfo.InvariantCulture);
            return str;
        }

        {
            var str = value.ToString();
            return str;
        }
    }

    /// <summary>
    ///     Конвертировать сложный объект в строку
    /// </summary>
    public static string ConvertComplexObject<TType>(
        TType value,
        string key,
        string format = null,
        JsonSerializerSettings serializerSettings = null)
    {
        const string separator = "&";

        if (!IsComplexObject(typeof(TType)))
        {
            return Convert(value, format, serializerSettings);
        }

        if (typeof(TType).IsCollection())
        {
            if (value is IList list)
            {
                return ConvertCollection(list);
            }

            if (value is Array array)
            {
                return ConvertCollection(array);
            }

            return Convert(value, format, serializerSettings);
        }
        else
        {
            var props = value.GetType().GetProperties();
            var representations = props.Select(prop =>
            {
                var propValue = prop.GetValue(value, null) ?? "null";
                var propKey = prop.GetCustomAttribute<FromQueryAttribute>()?.Name ?? prop.Name.ToLowerInvariant();
                var stringValue = IsComplexObject(prop.PropertyType) ? ConvertComplexObject(propValue, propKey, format, serializerSettings) : Convert(propValue, format, serializerSettings);

                return $"{propKey}={stringValue}";
            }).ToArray();

            return string.Join(separator, representations);
        }

        static bool IsComplexObject(Type type)
        {
            return !(type.IsPrimitive || type.IsEnum) && type.IsClass && !type.FullName.StartsWith("System.");
        }

        string ConvertCollection(IList list)
        {
            var commonValue = string.Empty;
            for (var i = 0; i < list.Count; i++)
            {
                var itemKey = $"{key}[{i}]";
                var itemValues = ConvertComplexObject(list[i], itemKey, format, serializerSettings).Split(separator);
                var itemValue = string.Join(separator, itemValues.Select(v => $"{itemKey}{v}"));

                commonValue += itemValue;
            }

            return commonValue;
        }
    }

    /// <summary>
    ///     Преобразовать значение в строку
    /// </summary>
    public static string Convert(
        object value,
        string format = null,
        JsonSerializerSettings serializerSettings = null)
    {
        if (value is null)
        {
            return "";
        }

        var type = value.GetType();
        if (Converters.TryGetValue(type, out var func))
        {
            var str = func.Convert(value, format);
            return str;
        }

        if (type.IsEnum)
        {
            var metadata = EnumMetadata.Get(type, serializerSettings);

            if (metadata.TryConvert(value, out var str))
            {
                return str;
            }
        }

        if (value is IFormattable formattable)
        {
            var str = formattable.ToString(format, CultureInfo.InvariantCulture);
            return str;
        }

        if (value is IConvertible convertible)
        {
            var str = convertible.ToString(CultureInfo.InvariantCulture);
            return str;
        }

        {
            var str = value.ToString();
            return str;
        }
    }
}