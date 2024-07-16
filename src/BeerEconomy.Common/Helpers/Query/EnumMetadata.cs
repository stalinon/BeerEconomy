using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BeerEconomy.Common.Helpers
{
    /// <summary>
    ///     Метаданные для перечисления
    /// </summary>
    internal sealed class EnumMetadata
    {
        /// <summary>
        ///     Метаданные для значения из перечисления
        /// </summary>
        public sealed class EnumValueMetadata
        {
            /// <summary>
            ///     .ctor
            /// </summary>
            public EnumValueMetadata(object value, string name)
            {
                Value = value;
                Name = name;
            }

            /// <summary>
            ///     Значение
            /// </summary>
            public object Value { get; }

            /// <summary>
            ///     Текстовое значение
            /// </summary>
            public string Name { get; }
        }

        private static readonly ConcurrentDictionary<Type, EnumMetadata> _enumMetadata = new();

        private readonly Dictionary<string, EnumValueMetadata> _names = new(StringComparer.Ordinal);
        private readonly Dictionary<long, EnumValueMetadata> _values = new();

        /// <summary>
        ///     Получить метаданные для перечисления
        /// </summary>
        public static EnumMetadata Get<T>(JsonSerializerSettings serializerSettings = null)
            where T : struct, Enum
        {
            return Get(typeof(T), serializerSettings);
        }

        /// <summary>
        ///     Получить метаданные для перечисления
        /// </summary>
        public static EnumMetadata Get(Type type, JsonSerializerSettings serializerSettings = null)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            type = underlyingType ?? type;

            if (!type.IsEnum)
            {
                throw new ArgumentException($"Type {type} is not an enum", nameof(type));
            }

            return _enumMetadata.GetOrAdd(type, t => new EnumMetadata(t, serializerSettings));
        }

        private EnumMetadata(Type type, JsonSerializerSettings serializerSettings = null)
        {
            IsFlags = type.GetCustomAttribute<FlagsAttribute>() != null;

            foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (field.FieldType != type)
                {
                    continue;
                }

                if (field.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                {
                    continue;
                }

                var value = field.GetValue(null);
                var intValue = Convert.ToInt64(value);

                var json = serializerSettings != null ? JsonConvert.SerializeObject(value, serializerSettings) : JsonConvert.SerializeObject(value);

                var token = (JValue)JToken.Parse(json);
                var str = token.Value?.ToString() ?? "";
                var metadata = new EnumValueMetadata(value, str);

                _names[str] = metadata;
                _values[intValue] = metadata;
            }

            Values = ImmutableArray.CreateRange(_names.Values);

            ErrorMessage = $"Expected one of the following values: {string.Join(", ", ValidValuesIterator())}";

            IEnumerable<string> ValidValuesIterator()
            {
                foreach (var (_, value) in _names)
                {
                    var str = $"'{value.Name}'";
                    yield return str;
                }
            }
        }

        /// <summary>
        ///     Является ли перечисление флагами
        /// </summary>
        public bool IsFlags { get; }

        /// <summary>
        ///     Значения перечисления
        /// </summary>
        public ImmutableArray<EnumValueMetadata> Values { get; }

        /// <summary>
        ///     Сообщение об ошибке "Неожиданное значение"
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        ///     Разобрать значение из строки
        /// </summary>
        public bool TryParse(string str, out object result)
        {
            if (!IsFlags)
            {
                if (!_names.TryGetValue(str, out var value))
                {
                    result = default;
                    return false;
                }

                result = value.Value;
                return true;
            }

            var accumulated = 0L;

            foreach (var part in str.Split(',', '+', ' ', ';'))
            {
                if (string.IsNullOrWhiteSpace(part))
                {
                    result = default;
                    return false;
                }

                if (!_names.TryGetValue(part, out var value))
                {
                    result = default;
                    return false;
                }

                accumulated |= Convert.ToInt64(value.Value);
            }

            result = accumulated;
            return true;
        }

        /// <summary>
        ///     Преобразовать значение в строку
        /// </summary>
        public bool TryConvert(object value, out string result)
        {
            var intValue = Convert.ToInt64(value);

            if (_values.TryGetValue(intValue, out var metadata))
            {
                result = metadata.Name;
                return true;
            }

            if (!IsFlags)
            {
                result = default;
                return false;
            }

            result = string.Join("+", FlagNamesIterator(intValue));
            return true;
        }

        private IEnumerable<string> FlagNamesIterator(long intValue)
        {
            var hasAny = false;
            foreach (var (key, metadata) in _values)
            {
                if ((key & intValue) != 0)
                {
                    intValue &= ~key;
                    hasAny = true;
                    yield return metadata.Name;
                }
            }

            if (!hasAny)
            {
                yield return "";
            }
        }
    }
}
