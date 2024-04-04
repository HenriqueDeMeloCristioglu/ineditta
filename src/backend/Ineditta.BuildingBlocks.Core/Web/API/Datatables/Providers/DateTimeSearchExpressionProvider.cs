using System;
using System.Globalization;
using System.Linq.Expressions;

namespace Ineditta.BuildingBlocks.Core.Web.API.Datatables.Providers
{
    public sealed class DateTimeSearchExpressionProvider : DefaultSearchExpressionProvider
    {
        public override ConstantExpression? GetValue(string input)
        {
            return !DateTime.TryParse(input, CultureInfo.CurrentCulture.DateTimeFormat, out DateTime value) ? default : Expression.Constant(value);
        }

        public override Expression GetComparison(MemberExpression left, string op, Expression right)
        {
            switch (op.ToLower(System.Globalization.CultureInfo.CurrentCulture))
            {
                case EqualsOperator:
                    if (Nullable.GetUnderlyingType(left.Type) != null)
                    {
                        var hasValueExpression = Expression.Property(left, "HasValue");
                        var valueExpression = Expression.Property(left, "Value");

                        // Truncate the time part of the DateTime
                        var truncatedValueExpression = Expression.Property(valueExpression, "Date");
                        var truncatedRightExpression = Expression.Property(right, "Date");

                        var expression = Expression.Condition(
                            hasValueExpression,
                            Expression.Equal(truncatedValueExpression, truncatedRightExpression),
                            Expression.Constant(false) // or Expression.Constant(null, typeof(bool)) if needed
                        );

                        return expression;
                    }

                    // Handle non-nullable DateTime
                    // Truncate the time part of the DateTime
                    var truncatedLeft = Expression.Property(left, "Date");
                    var truncatedRight = Expression.Property(right, "Date");

                    return Expression.Equal(truncatedLeft, truncatedRight);

                default: throw new ArgumentException($"Invalid Operator '{op}'.");
            }
        }
    }
}