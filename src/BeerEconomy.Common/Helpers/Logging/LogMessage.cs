using System.Text;

namespace BeerEconomy.Common.Helpers.Logging;

/// <summary>
///     Сообщение для логирования
/// </summary>
public sealed class LogMessage
{
    private StringBuilder _message;

    /// <inheritdoc cref="LogMessage"/>
    public LogMessage()
    {
        _message = new();
    }

    /// <inheritdoc cref="LogMessage"/>
    public LogMessage(string message)
    {
        _message = new(message);
        _message.AppendLine(string.Empty);
    }

    /// <summary>
    ///     Добавить
    /// </summary>
    public LogMessage Append(string message)
    {
        _message.AppendLine(message);
        return this;
    }

    /// <summary>
    ///     Обновилось свойство
    /// </summary>
    public LogMessage PropertyChanged<TProperty>(string propName, TProperty oldValue, TProperty newValue)
    {
        return Property(propName, $"`{oldValue}` -> `{newValue}`");
    }

    /// <summary>
    ///     Свойство
    /// </summary>
    public LogMessage Property<TProperty>(string propName, TProperty oldValue)
    {
        return Append($"{propName}: {oldValue}");
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return _message.ToString();
    }

    /// <summary>
    ///     Неявное преобразование к строке
    /// </summary>
    public static implicit operator string(LogMessage logMessage)
    {
        return logMessage.ToString();
    }
}