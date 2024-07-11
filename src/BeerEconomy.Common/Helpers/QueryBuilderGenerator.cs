using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BeerEconomy.Common.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Scraper.Common.Models
{
    /// <summary>
    ///     Генерирует объект типа <see cref="QueryBuilder"/> из объекта запроса
    /// </summary>
    internal static class QueryBuilderGenerator
    {
        private delegate QueryBuilder FactoryFunc(object query, QueryStringOptions? options = null, JsonSerializerSettings serializerSettings = null);

        private static readonly ConcurrentDictionary<Type, FactoryFunc> _generatorFunctions = new();

        /// <summary>
        ///     Сгенерировать построитель запроса из объекта запроса
        /// </summary>
        public static QueryBuilder Generate(object query, QueryStringOptions? options = null, JsonSerializerSettings serializerSettings = null)
        {
            if (query == null)
            {
                return QueryBuilder.New(options, serializerSettings);
            }

            var func = GetOrCreateFactoryFunc(query.GetType());
            var builder = func(query, options, serializerSettings);
            return builder;
        }

        private static FactoryFunc GetOrCreateFactoryFunc(Type type)
        {
            return _generatorFunctions.GetOrAdd(type, CreateFactoryFunc);
        }

        // Генерирует функцию вида:
        //
        //   var query = (TYPE)obj;
        //   var builder = new QueryBuilder();
        //   builder.AddGeneric("NAME_1", query.FIELD_1);
        //   builder.AddGeneric("NAME_2", query.FIELD_2);
        //   builder.AddGeneric("NAME_N", query.FIELD_N);
        //   return builder;
        private static FactoryFunc CreateFactoryFunc(Type type)
        {
            /*
             * Генерирует
             * obj => {
             *      var query = (TYPE)obj;
             *      var builder = new QueryBuilder();
             *      builder.AddGeneric("NAME", query.FIELD);// for each field
             *      return builder;
             * }
             *
             */

            var objParameter = Expression.Parameter(typeof(object), "obj");
            var optionsParameter = Expression.Parameter(typeof(QueryStringOptions?), "options");
            var serializerSettingsParameter = Expression.Parameter(typeof(JsonSerializerSettings), "serializerSettings");
            var builderVariable = Expression.Variable(typeof(QueryBuilder), "builder");
            var queryVariable = Expression.Variable(type, "query");

            var body = Expression.Block(
                new[] { builderVariable, queryVariable, },
                CreateFuncIterator(type, objParameter, optionsParameter, serializerSettingsParameter, builderVariable, queryVariable)
            );

            var lambda = Expression.Lambda<FactoryFunc>(
                body,
                objParameter,
                optionsParameter,
                serializerSettingsParameter
            );
            var func = lambda.Compile();
            return func;
        }

        private static IEnumerable<Expression> CreateFuncIterator(
            Type type,
            ParameterExpression objParameter,
            ParameterExpression optionsParameter,
            ParameterExpression serializerSettingsParameter,
            ParameterExpression builderVariable,
            ParameterExpression queryVariable)
        {
            var returnTarget = Expression.Label(typeof(QueryBuilder));
            var returnExpression = Expression.Return(returnTarget, builderVariable, typeof(QueryBuilder));
            var returnLabel = Expression.Label(returnTarget, Expression.Default(typeof(QueryBuilder)));

            // query = (TYPE)obj;
            yield return Expression.Assign(queryVariable, Expression.Convert(objParameter, type));

            // builder = new QueryBuilder();
            var method = typeof(QueryBuilder).GetMethod(nameof(QueryBuilder.New));
            var builderInstance = Expression.Call(method!, optionsParameter, serializerSettingsParameter);
            yield return Expression.Assign(builderVariable, builderInstance);

            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var expr = GetValueBinder(builderVariable, queryVariable, property);
                if (expr != null)
                {
                    yield return expr;
                }
            }

            yield return returnExpression;
            yield return returnLabel;
        }

        private static readonly MethodInfo AddStringMethod = GetMethod(nameof(QueryBuilder.AddString));
        private static readonly MethodInfo AddGenericMethod = GetMethod(nameof(QueryBuilder.AddGeneric));
        private static readonly MethodInfo AddNullableGenericMethod = GetMethod(nameof(QueryBuilder.AddNullableGeneric));
        private static readonly MethodInfo AddArrayMethod = GetMethod(nameof(QueryBuilder.AddArray));
        private static readonly MethodInfo AddEnumerableMethod = GetMethod(nameof(QueryBuilder.AddEnumerable));

        private static MethodInfo GetMethod(Type type, string name, BindingFlags bindingFlags)
        {
            return type.GetMethod(name, bindingFlags | BindingFlags.Public | BindingFlags.NonPublic) ??
                   throw new Exception($"Method {type}::{name}() is not found");
        }

        private static MethodInfo GetMethod(string name)
        {
            return GetMethod(typeof(QueryBuilder), name, BindingFlags.Instance);
        }

        private static MethodCallExpression GetValueBinder(Expression builderVariable, Expression queryVariable, PropertyInfo property)
        {
            var fieldName = GetQueryFieldName(property);
            if (string.IsNullOrEmpty(fieldName))
            {
                return null;
            }

            var fieldNameConst = Expression.Constant(fieldName, typeof(string));

            if (property.PropertyType == typeof(string))
            {
                // builder.AddString(NAME, query.FIELD);
                return Expression.Call(
                    builderVariable,
                    AddStringMethod,
                    fieldNameConst,
                    Expression.MakeMemberAccess(queryVariable, property)
                );
            }

            var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);
            if (underlyingType != null)
            {
                // builder.AddNullableGeneric<T>(NAME, query.FIELD);

                return Expression.Call(
                    builderVariable,
                    AddNullableGenericMethod.MakeGenericMethod(underlyingType),
                    fieldNameConst,
                    Expression.MakeMemberAccess(queryVariable, property)
                );
            }

            if (property.PropertyType.IsArray)
            {
                // builder.AddArray<T>(NAME, query.FIELD);
                return Expression.Call(
                    builderVariable,
                    AddArrayMethod.MakeGenericMethod(property.PropertyType.GetElementType()!),
                    fieldNameConst,
                    Expression.MakeMemberAccess(queryVariable, property)
                );
            }

            var iface = property.PropertyType.GetInterfaces()
                .FirstOrDefault(_ => _.IsGenericType && _.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (iface != null)
            {
                var elementType = iface.GenericTypeArguments[0];

                // builder.AddEnumerable<T>(NAME, (IEnumerable<T>) query.FIELD);
                var field = Expression.MakeMemberAccess(queryVariable, property);

                return Expression.Call(
                    builderVariable,
                    AddEnumerableMethod.MakeGenericMethod(elementType),
                    fieldNameConst,
                    Expression.Convert(field, iface)
                );
            }

            // builder.AddGeneric<T>(NAME, query.FIELD);
            return Expression.Call(
                builderVariable,
                AddGenericMethod.MakeGenericMethod(property.PropertyType),
                fieldNameConst,
                Expression.MakeMemberAccess(queryVariable, property)
            );
        }

        private static string GetQueryFieldName(PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes().ToArray();

            var bindNeverAttribute = attributes.OfType<BindNeverAttribute>().FirstOrDefault();
            if (bindNeverAttribute != null)
            {
                return null;
            }

            var bindingSourceMetadata = attributes.OfType<IBindingSourceMetadata>().FirstOrDefault();
            if (bindingSourceMetadata != null && bindingSourceMetadata.BindingSource != BindingSource.Query)
            {
                return null;
            }

            var modelNameProvider = attributes.OfType<IModelNameProvider>().FirstOrDefault();
            if (!string.IsNullOrEmpty(modelNameProvider?.Name))
            {
                return modelNameProvider.Name;
            }

            return property.Name;
        }
    }
}
