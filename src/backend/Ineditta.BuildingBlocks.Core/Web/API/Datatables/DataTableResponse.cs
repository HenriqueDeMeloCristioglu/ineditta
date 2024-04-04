namespace Ineditta.BuildingBlocks.Core.Web.API.Datatables
{
    public sealed class DataTableResponse<T> where T : class
    {
        public int TotalCount { get; private set; }
        public IEnumerable<T>? Items { get; private set; }

        public DataTableResponse(IEnumerable<T> items, int count)
        {
            TotalCount = count;
            Items = items;
        }

        public DataTableResponse(IEnumerable<T> items)
        {
            TotalCount = items?.Count() ?? 0;
            Items = items;
        }
    }
}
