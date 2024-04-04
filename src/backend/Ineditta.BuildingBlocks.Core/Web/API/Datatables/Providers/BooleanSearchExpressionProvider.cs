using System.Linq.Expressions;

namespace Ineditta.BuildingBlocks.Core.Web.API.Datatables.Providers
{
    public class BooleanSearchExpressionProvider : DefaultSearchExpressionProvider
    {
        public override ConstantExpression? GetValue(string input)
        {
            return !bool.TryParse(input, out var value) ? default : Expression.Constant(value);
        }

        public override Expression GetComparison(MemberExpression left, string op, Expression right)
        {
            switch (op.ToLowerInvariant())
            {
                case EqualsOperator:
                    if (Nullable.GetUnderlyingType(left.Type) != null)
                    {
                        var expression = Expression.Call(left, "GetValueOrDefault", Type.EmptyTypes);
                        return Expression.Equal(expression, Expression.Convert(right, typeof(bool)));
                    }
                    return Expression.Equal(left, Expression.Convert(right, typeof(bool)));

                default: throw new ArgumentException($"Invalid Operator '{op}'.");
            }
        }
    }
}
