using System.Diagnostics;

using Ineditta.BuildingBlocks.Core.Database.Interceptors;
using Ineditta.BuildingBlocks.Core.Extensions;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Ineditta.BuildingBlocks.Core.Web.API.Datatables
{
    public static class DataTableExtension
    {
        public static async ValueTask<DataTableResponse<T>> ToDataTableResponseAsync<T>(this IQueryable<T> query, DataTableRequest request, bool searchable = true) where T : class
        {
            var queryFiltered = query;

            var columns = string.IsNullOrEmpty(request.Columns) ? Enumerable.Empty<string>() : request.Columns.Split(',').Select(column => column.Trim());

            if (searchable && !string.IsNullOrEmpty(request.Filter))
            {
                queryFiltered = queryFiltered.DynamicSearch(request.Filter, columns);
            }

            if (!string.IsNullOrEmpty(request.SortColumn) && !string.IsNullOrEmpty(request.SortOrder))
            {
                queryFiltered = queryFiltered!.DynamicOrder(request.SortColumn, request.SortOrder);
            }

            if (request.SortColumns is not null && request.SortColumns.Any())
            {
                queryFiltered = queryFiltered!.DynamicOrder(request.SortColumns);
            }

            var count = await Task.FromResult(queryFiltered!.Count());

            var result = await Task.FromResult(request.ShowAllRecords
                                               ? queryFiltered!.ToList()
                                               : queryFiltered!.Skip(request.PageNumber * request.PageSize).Take(request.GetPageSize()).ToList());

            return new DataTableResponse<T>(result, count);
        }

        public static async ValueTask<DataTableResponse<T>> ToDataTableV2ResponseAsync<T>(this IQueryable<T> query, DbContext context, DataTableRequest request, bool searchable = true) where T : class
        {
            var queryFiltered = query;

            var columns = string.IsNullOrEmpty(request.Columns) ? Enumerable.Empty<string>() : request.Columns.Split(',').Select(column => column.Trim());

            if (searchable && !string.IsNullOrEmpty(request.Filter))
            {
                queryFiltered = queryFiltered.DynamicSearch(request.Filter, columns);
            }

            if (!string.IsNullOrEmpty(request.SortColumn) && !string.IsNullOrEmpty(request.SortOrder))
            {
                queryFiltered = queryFiltered!.DynamicOrder(request.SortColumn, request.SortOrder);
            }

            if (request.SortColumns is not null && request.SortColumns.Any())
            {
                queryFiltered = queryFiltered!.DynamicOrder(request.SortColumns);
            }

            var hasSqlCalcFoundRows = queryFiltered!.ToQueryString().Contains(InterceptorTag.SqlCalcFoundRows, StringComparison.CurrentCultureIgnoreCase);

            var result = await Task.FromResult(request.ShowAllRecords
                                               ? queryFiltered!.ToList()
                                               : queryFiltered!.Skip(request.PageNumber * request.PageSize).Take(request.GetPageSize()).ToList());

            var count = hasSqlCalcFoundRows ?
                        await context.Database.SqlQueryRaw<int>("SELECT FOUND_ROWS() as Value").FirstOrDefaultAsync() :
                        await Task.FromResult(queryFiltered!.Count());

            return new DataTableResponse<T>(result, count);
        }

        public static async ValueTask<DataTableResponse<IDictionary<string, object>>> DataTable(this IEnumerable<IDictionary<string, object>> models, DataTableRequest request)
        {
            if (!string.IsNullOrEmpty(request.SortColumn))
            {
                models = request.SortOrder == "asc" ? models.OrderBy(r => r[request.SortColumn]) : models.OrderByDescending(r => r[request.SortColumn]);
            }

            var datatable = await Task.FromResult(request.ShowAllRecords
                                               ? models
                                               : models.Skip(request.PageNumber * request.PageSize).Take(request.GetPageSize()).ToList());

            return new DataTableResponse<IDictionary<string, object>>(datatable, models.Count());
        }
    }
}
