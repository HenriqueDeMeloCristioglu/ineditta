using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Attributes;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Providers;

using MailKit.Search;

namespace Ineditta.BuildingBlocks.Core.Extensions
{
    public static class QueryableExtension
    {
        public static IOrderedQueryable<T> DynamicOrder<T>(this IQueryable<T> source, string sortColumn, string sortOrder = "asc")
        {
            if (string.IsNullOrEmpty(sortColumn))
                return (IOrderedQueryable<T>)source;

            var propertyName = typeof(T)
                                .GetProperties()
                                .SingleOrDefault(x => string.Equals(x.Name, sortColumn, StringComparison.OrdinalIgnoreCase));

            if (propertyName == null)
                return (IOrderedQueryable<T>)source;

            var param = Expression.Parameter(typeof(T), string.Empty);
            var property = Expression.PropertyOrField(param, propertyName.Name);
            var sort = Expression.Lambda(property, param);

            var call = Expression.Call(
                typeof(Queryable),
                ("OrderBy" +
                (sortOrder == "desc" ? "Descending" : string.Empty)),
                new[] { typeof(T), property.Type },
                source.Expression,
                Expression.Quote(sort));

            return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
        }

        public static IOrderedQueryable<T> DynamicOrder<T>(this IQueryable<T> source, IEnumerable<KeyValuePair<string, string>> sortColumnsOrders)
        {
            if (sortColumnsOrders is null || !sortColumnsOrders.Any())
            {
                return (IOrderedQueryable<T>)source;
            }

            source = (IOrderedQueryable<T>)source;

            foreach (var sortColumnOrder in sortColumnsOrders)
            {
                if (string.IsNullOrEmpty(sortColumnOrder.Key))
                {
                    continue;
                }

                var propertyName = typeof(T)
                                    .GetProperties()
                                    .SingleOrDefault(x => string.Equals(x.Name, sortColumnOrder.Key, StringComparison.OrdinalIgnoreCase));

                if (propertyName == null)
                {
                    continue;
                }

                var param = Expression.Parameter(typeof(T), string.Empty);
                var property = Expression.PropertyOrField(param, propertyName.Name);
                var sort = Expression.Lambda(property, param);

                var call = Expression.Call(
                    typeof(Queryable),
                ("ThenBy" +
                    (sortColumnOrder.Value == "desc" ? "Descending" : string.Empty)),
                    new[] { typeof(T), property.Type },
                    source.Expression,
                    Expression.Quote(sort));

                source = (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
            }

            return (IOrderedQueryable<T>)source;
        }

        private static IQueryable<T>? DynamicWhere<T>(this IQueryable<T> query, LambdaExpression predicate)
        {
            var queryableMethods = typeof(Queryable).GetMethods().ToArray();

            var whereMethodBuilder = queryableMethods
                .First(x => x.Name == "Where" && x.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T));

            return whereMethodBuilder is null ? default : (IQueryable<T>?)whereMethodBuilder.Invoke(null, new object[] { query, predicate });
        }

        public static IQueryable<T>? DynamicSearch<T>(this IQueryable<T> query, string textSearch, IEnumerable<string> columns) where T : class
        {
            if (string.IsNullOrEmpty(textSearch))
                return query;

            var lambdaExpression = CreateLambdaExpression<T>(textSearch, columns);

            return lambdaExpression is null ? query : query.DynamicWhere(lambdaExpression);
        }

        private static LambdaExpression? CreateLambdaExpression<T>(string filter, IEnumerable<string> columns) where T : class
        {
            var stringSearchProvider = new StringSearchExpressionProvider();
            var intSearchProvider = new IntSearchExpressionProvider();
            var longSearchProvider = new LongSearchExpressionProvider();
            var dateTimeSearchProvider = new DateTimeSearchExpressionProvider();
            var dateOnlySearchProvider = new DateOnlySearchExpressionProvider();
            var decimalSeachProvider = new DecimalSearchExpressionProvider();
            var booleanSearchProvider = new BooleanSearchExpressionProvider();

            var properties = typeof(T)
                            .GetProperties()
                            .Where(p => p.CanWrite && Attribute.GetCustomAttribute(p, typeof(NotSearchableDataTableAttribute)) is not NotSearchableDataTableAttribute);

            if (columns.Any())
            {
                properties = properties.Where(property => columns.Any(column => column.ToLowerInvariant() == property.Name.ToLowerInvariant()));
            }

            var parameter = Expression.Parameter(typeof(T), $"{typeof(T).Name}Search");

            Expression? finalExpression = null;

            properties.ToList().ForEach(p =>
            {
                var memberExpression = Expression.Property(parameter, p);

                Expression? expression;

                if (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))
                {
                    expression = dateTimeSearchProvider.EqualsExpression(memberExpression, filter);
                }
                else if(p.PropertyType == typeof(DateOnly) || p.PropertyType == typeof(DateOnly?))
                {
                    expression = dateOnlySearchProvider.EqualsExpression(memberExpression, filter);
                }
                else if (p.PropertyType == typeof(string))
                {
                    expression = stringSearchProvider.StartsWithExpression(memberExpression, filter);
                }
                else if (p.PropertyType == typeof(decimal) || p.PropertyType == typeof(double)
                     || p.PropertyType == typeof(decimal?) || p.PropertyType == typeof(double?))
                {
                    expression = decimalSeachProvider.EqualsExpression(memberExpression, filter);
                }
                else if (p.PropertyType == typeof(bool) || p.PropertyType == typeof(bool?))
                {
                    expression = booleanSearchProvider.EqualsExpression(memberExpression, filter);
                }
                else if (p.PropertyType == typeof(long) || p.PropertyType == typeof(long?))
                {
                    expression = longSearchProvider.EqualsExpression(memberExpression, filter);
                }
                else
                {
                    expression = intSearchProvider.EqualsExpression(memberExpression, filter);
                }

                if (expression == null)
                    return;

                if (finalExpression == null)
                {
                    finalExpression = expression;
                    return;
                }

                finalExpression = Expression.OrElse(finalExpression, expression);
            });

            return ReflectionExtension.GetLambda<T, bool>(parameter, finalExpression);
        }

        public static IQueryable<dynamic> DynamicSelect<T>(this IQueryable<T> query, List<string> properties)
        {
            return query.Select(x => CreateDynamicObject(x, properties.Select(c => c).ToList()));
        }

        private static dynamic CreateDynamicObject<T>(T item, List<string> properties)
        {
            var expando = new ExpandoObject() as IDictionary<string, object>;

            foreach (var property in properties)
            {
                var propInfo = typeof(T).GetProperty(property, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propInfo != null)
                {
#pragma warning disable CS8601 // Possible null reference assignment.
                    expando[property] = propInfo?.GetValue(item, null);
#pragma warning restore CS8601 // Possible null reference assignment.
                }
            }

            return expando;
        }
    }
}
