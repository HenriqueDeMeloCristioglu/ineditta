using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Ineditta.BuildingBlocks.Core.Extensions
{
    public static class ReflectionExtension
    {
        public static void SetValueProperty<T>(T item, string propertyName, string value)
        {
            var property = typeof(T).GetProperty(propertyName) ?? throw new ArgumentException($"Property {propertyName} not found", nameof(propertyName));

            var converter = TypeDescriptor.GetConverter(property.PropertyType);

            if (converter != null)
            {
                var newValue = converter.ConvertFromString(value);

                item!.GetType()!.GetProperty(propertyName)!.SetValue(item, newValue, null);
            }
        }

        public static LambdaExpression? GetLambda<TSource, TDest>(ParameterExpression obj, Expression? arg)
        {
            return arg is null ? default : GetLambda(typeof(TSource), typeof(TDest), obj, arg!);
        }

        private static LambdaExpression? GetLambda(Type source, Type dest, ParameterExpression obj, Expression arg)
        {
            var lambdaBuilder = GetLambdaFuncBuilder(source, dest);

            return (LambdaExpression?)lambdaBuilder.Invoke(null, new object[] { arg, new[] { obj } });
        }

        private static MethodInfo GetLambdaFuncBuilder(Type source, Type dest)
        {
            var predicate = typeof(Func<,>).MakeGenericType(source, dest);
            return LambdaMethod.MakeGenericMethod(predicate);
        }

        public static IEnumerable<PropertyInfo> GetProperties<T>(string[] properties) where T : new()
        {
            var result = typeof(T).GetProperties();

            return !result.Any() ? Enumerable.Empty<PropertyInfo>() : result.Where(x => properties.Contains(x.Name));
        }

        public static IEnumerable<PropertyInfo> GetPropertiesInfo<T>(Func<T, string[]> function) where T : new()
        {
            var type = typeof(T);

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            return function.Invoke(new T()).Select(type.GetProperty).Where(t => t != null)
                ?? Enumerable.Empty<PropertyInfo>();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }

        private static readonly MethodInfo LambdaMethod = typeof(Expression)
         .GetMethods()
         .First(x => x.Name == "Lambda" && x.ContainsGenericParameters && x.GetParameters().Length == 2);
    }
}
