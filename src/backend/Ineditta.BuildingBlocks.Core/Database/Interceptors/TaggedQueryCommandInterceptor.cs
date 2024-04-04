using System.Data.Common;

using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Ineditta.BuildingBlocks.Core.Database.Interceptors
{
    public class TaggedQueryCommandInterceptor : DbCommandInterceptor
    {
        public override InterceptionResult<DbDataReader> ReaderExecuting(
       DbCommand command,
       CommandEventData eventData,
       InterceptionResult<DbDataReader> result)
        {
            ManipulateCommand(command);

            return result;
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            ManipulateCommand(command);

            return new ValueTask<InterceptionResult<DbDataReader>>(result);
        }

        private static void ManipulateCommand(DbCommand command)
        {
            if (command.CommandText.Contains(InterceptorTag.SqlCalcFoundRows, StringComparison.Ordinal))
            {
                int position = command.CommandText.IndexOf("SELECT", StringComparison.Ordinal);

                command.CommandText = command.CommandText.Insert(position + 6, " SQL_CALC_FOUND_ROWS ");
            }
        }
    }
}
