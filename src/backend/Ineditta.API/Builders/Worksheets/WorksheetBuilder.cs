using ClosedXML.Excel;

namespace Ineditta.API.Builders.Worksheets
{
    public static class WorksheetBuilder
    {
        public static XLWorkbook Create()
        {
            return new XLWorkbook();
        }
        public static IXLWorksheet AddWorkSheet(this XLWorkbook workbook, string worksheetName)
        {
            return workbook.Worksheets.Add(worksheetName);
        }
        public static XLWorkbook ReadFrom(string path)
        {
            var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return new XLWorkbook(file);
        }

        public static IXLWorksheet GetFirst(this XLWorkbook workbook)
        {
            return workbook.Worksheets.FirstOrDefault()!;
        }

        public static byte[] ToByteArray(this IXLWorksheet _, XLWorkbook workbook)
        {
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            workbook?.Dispose();

            return stream.ToArray();
        }

        public static IXLWorksheet Build(this IXLWorksheet worksheet, Action<IXLWorksheet> action)
        {
            action(worksheet);

            return worksheet;
        }
    }
}
