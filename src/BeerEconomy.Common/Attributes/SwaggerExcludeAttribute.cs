namespace BeerEconomy.Common.Attributes;

/// <summary>
///     Убрать свойство из сваггера
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class SwaggerExcludeAttribute : Attribute;