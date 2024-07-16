namespace BeerEconomy.Common.Helpers.Exceptions;

/// <summary>
///     Внутренняя ошибка
/// </summary>
public class InternalException : Exception
{
    /// <summary>
    ///     Код ошибки
    /// </summary>
    public ErrorCode ErrorCode { get; }

    /// <inheritdoc cref="InternalException"/>
    public InternalException(ErrorCode errorCode)
        : base()
    {
        ErrorCode = errorCode;
    }

    /// <inheritdoc cref="InternalException"/>
    public InternalException(ErrorCode errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <inheritdoc cref="InternalException"/>
    public InternalException(ErrorCode errorCode, string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}