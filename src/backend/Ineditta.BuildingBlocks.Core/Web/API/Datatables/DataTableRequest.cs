namespace Ineditta.BuildingBlocks.Core.Web.API.Datatables
{
    public class DataTableRequest
    {
        public string? Filter { get; set; }
        public string? SortColumn { get; set; }
        public string? SortOrder { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int GetPageSize() => PageSize > 0 && PageSize < 3000 ? PageSize : 10;
        public bool ShowAllRecords { get; set; }
        public string? Columns { get; set; }
        public IEnumerable<KeyValuePair<string, string>>? SortColumns { get; set; }
    }
}
