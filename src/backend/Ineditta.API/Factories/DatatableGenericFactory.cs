using System.Text;

using Ineditta.BuildingBlocks.Core.Extensions;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables;
using Ineditta.Repository.Contexts;

namespace Ineditta.API.Factories
{
    public class DatatableGenericFactory
    {
        private readonly InedittaDbContext _context;
        public DatatableGenericFactory(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask<DataTableResponse<T>> Criar<T>(DataTableRequest request, StringBuilder query, Dictionary<string, object> parameters, bool searchable = true) where T : class
        {
            var queryFiltered = new StringBuilder(@"
                SELECT i.* FROM (" +
                    query.ToString() +
                @") i
                WHERE TRUE"    
            );

            var parametersCount = 0;

            var columns = string.IsNullOrEmpty(request.Columns) ? Enumerable.Empty<string>() : request.Columns.Split(',').Select(column => column.Trim());

            if (searchable && !string.IsNullOrEmpty(request.Filter))
            {
                foreach (var column in columns)
                {
                    queryFiltered.Append(@" AND (FALSE ");

                    queryFiltered.Append(@" OR i.@column " + parametersCount + "LIKE '%@filterValue%' ");
                    parameters.Add("@column" + parametersCount, column);

                    queryFiltered.Append(@") ");
                }
            }

            if (!string.IsNullOrEmpty(request.SortColumn) && !string.IsNullOrEmpty(request.SortOrder))
            {
                string order = "";
                if (request.SortOrder.ToUpperInvariant() == "DESC") order = request.SortOrder;
                if (request.SortOrder.ToUpperInvariant() == "ASC") order = request.SortOrder;

                queryFiltered.Append(@" ORDER BY @orderColumn " + order);
                parameters.Add("@orderColumn", request.SortColumn.ToPascalCase());
            }

            var count = (await _context.SelectFromRawSqlAsync<T>(queryFiltered.ToString(), parameters)).Count();
            var items = await _context.SelectFromRawSqlAsync<T>(queryFiltered.ToString(), parameters);

            var datatableResult = new DataTableResponse<T>(items, count);
            return datatableResult;
        }
    }
}
