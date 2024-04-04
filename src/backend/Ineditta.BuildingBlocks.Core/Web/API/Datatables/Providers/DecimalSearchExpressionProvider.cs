using System.Linq.Expressions;

namespace Ineditta.BuildingBlocks.Core.Web.API.Datatables.Providers
{
    public sealed class DecimalSearchExpressionProvider: DefaultSearchExpressionProvider
    {
        public override ConstantExpression? GetValue(string input)
        {
            return !decimal.TryParse(input, out var value) ? default : Expression.Constant(value);
        }
    }
}
