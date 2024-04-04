using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Contracts;

using System.Linq.Expressions;

namespace Ineditta.BuildingBlocks.Core.Web.API.Datatables.Providers
{
    public abstract class DefaultSearchExpressionProvider : ISearchExpressionProvider
    {
        public const string EqualsOperator = "eq";

        public virtual IEnumerable<string> GetOperators()
        {
            yield return EqualsOperator;
        }

        public virtual ConstantExpression? GetValue(string input)
        {
            return Expression.Constant(input);
        }

        public virtual Expression GetComparison(MemberExpression left, string op, Expression right)
        {
            switch (op.ToLower(System.Globalization.CultureInfo.CurrentCulture))
            {
                case EqualsOperator:
                    if (Nullable.GetUnderlyingType(left.Type) != null)
                    {
                        var expression = Expression.Call(left, "GetValueOrDefault", Type.EmptyTypes);
                        return Expression.Equal(expression, right);
                    }

                    return Expression.Equal(left, right);

                default: throw new ArgumentException($"Invalid Operator '{op}'.");
            }
        }

        public Expression? EqualsExpression(MemberExpression left, string filter)
        {
            var expression = GetValue(filter);

            return expression == null ? default : GetComparison(left, EqualsOperator, expression);
        }
    }
}
