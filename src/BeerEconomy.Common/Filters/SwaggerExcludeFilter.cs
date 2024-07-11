using System.Reflection;
using BeerEconomy.Common.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BeerEconomy.Common.Filters;

/// <summary>
///     Фильтр для вырезания определенных полей из Сваггера
/// </summary>
internal sealed class SwaggerExcludeFilter : ISchemaFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsPrimitive || schema.Properties == null || schema.Properties.Count == 0)
        {
            return;
        }

        var excludedProperties = context.Type.GetProperties()
            .Where(t => 
                t.GetCustomAttribute<SwaggerExcludeAttribute>() 
                != null);

        foreach (var excludedProperty in excludedProperties)
        {
            if (schema.Properties.ContainsKey(excludedProperty.Name))
                schema.Properties.Remove(excludedProperty.Name);
        }
    }
}