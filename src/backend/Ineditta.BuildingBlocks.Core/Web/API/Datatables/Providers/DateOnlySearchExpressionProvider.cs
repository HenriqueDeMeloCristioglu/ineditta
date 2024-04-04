using System.Globalization;
using System.Linq.Expressions;

namespace Ineditta.BuildingBlocks.Core.Web.API.Datatables.Providers
{
    public class DateOnlySearchExpressionProvider : DefaultSearchExpressionProvider
    {
        public override ConstantExpression? GetValue(string input)
        {
            return !DateOnly.TryParse(input, CultureInfo.CurrentCulture, out DateOnly value) ? default : Expression.Constant(value);
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

                        var expression = Expression.Condition(
                            hasValueExpression,
                            Expression.Equal(valueExpression, right),
                            Expression.Constant(false) // or Expression.Constant(null, typeof(bool)) if needed
                        );

                        return expression;
                    }

                    return Expression.Equal(left, right);

                default: throw new ArgumentException($"Invalid Operator '{op}'.");
            }
        }
    }
}
