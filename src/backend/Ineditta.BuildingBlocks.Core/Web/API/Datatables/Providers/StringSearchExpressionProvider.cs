using Microsoft.EntityFrameworkCore;

using System.Linq.Expressions;
using System.Reflection;

namespace Ineditta.BuildingBlocks.Core.Web.API.Datatables.Providers
{
    public sealed class StringSearchExpressionProvider : DefaultSearchExpressionProvider
    {
        private const string ContainsOperator = "co";
        private const string StartsWithOperatorConstant = "sw";
        private const string EqualsOperatorConstant = "eq";

        private static readonly MethodInfo StringEqualsMethod = typeof(string)
            .GetMethods()
            .First(x => x.Name == "Equals" && x.GetParameters().Length == 2);

        private static readonly MethodInfo ContainsMethod = typeof(string)
            .GetMethods()
            .First(x => x.Name == "Contains" && x.GetParameters().Length == 1);

        private static readonly MethodInfo ToLowerMethod = typeof(string)
            .GetMethods()
            .First(x => x.Name == "ToLower");

#pragma warning disable CS8601 // Possible null reference assignment.
        private static readonly MethodInfo StartsWithMethod = typeof(DbFunctionsExtensions)
           .GetMethod("Like", new[] { typeof(DbFunctions), typeof(string), typeof(string) });
#pragma warning restore CS8601 // Possible null reference assignment.

        private static ConstantExpression IgnoreCase
            => Expression.Constant(StringComparison.OrdinalIgnoreCase);

        public override IEnumerable<string> GetOperators()
        {
            return base.GetOperators()
                           .Concat(
                               new[]
                               {
                        StartsWithOperatorConstant,
                        ContainsOperator
                               });
        }

        public override Expression GetComparison(MemberExpression left, string op, Expression right)
        {
            return op.ToLowerInvariant() switch
            {
                StartsWithOperatorConstant => StartWithOperatorExpression(left, right),
                ContainsOperator => Expression.Call(Expression.Call(left, ToLowerMethod), ContainsMethod, right),
                EqualsOperatorConstant => Expression.Call(Expression.Call(left, ToLowerMethod), StringEqualsMethod, right, IgnoreCase),
                _ => base.GetComparison(left, op, right),
            };
        }

        public Expression StartsWithExpression(MemberExpression left, string filter)
        {
            var expression = GetValue('%' + filter + '%') ?? Expression.Constant(filter);

            return GetComparison(left, StartsWithOperatorConstant, expression);
        }

        private static Expression StartWithOperatorExpression(MemberExpression left, Expression right)
        {
            return Expression.Call(null, StartsWithMethod, Expression.Constant(EF.Functions), left, right);
        }
    }
}