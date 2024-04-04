using System.Collections.Generic;
using System.Linq.Expressions;

namespace Ineditta.BuildingBlocks.Core.Web.API.Datatables.Contracts
{
    public interface ISearchExpressionProvider
    {
        IEnumerable<string> GetOperators();

        ConstantExpression? GetValue(string input);

        Expression GetComparison(
            MemberExpression left,
            string op,
            Expression right);
    }
}
