using System.Linq.Expressions;

namespace Ineditta.BuildingBlocks.Core.Web.API.Datatables.Providers
{
    public sealed class LongSearchExpressionProvider : DefaultSearchExpressionProvider
    {
        public override ConstantExpression? GetValue(string input)
        {
            return !int.TryParse(input, out var value) ? default : Expression.Constant(value);
        }

        public override Expression GetComparison(MemberExpression left, string op, Expression right)
        {
            switch (op.ToLowerInvariant())
            {
                case EqualsOperator:
                    if (Nullable.GetUnderlyingType(left.Type) != null)
                    {
                        var teste = Expression.Call(left, "GetValueOrDefault", Type.EmptyTypes);
                        return Expression.Equal(teste, Expression.Convert(right, typeof(long)));
                    }
                    return Expression.Equal(left, Expression.Convert(right, typeof(long)));

                default: throw new ArgumentException($"Invalid Operator '{op}'.");
            }
        }
    }
}
