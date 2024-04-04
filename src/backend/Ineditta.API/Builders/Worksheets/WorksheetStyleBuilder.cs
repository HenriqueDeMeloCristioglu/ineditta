using ClosedXML.Excel;

namespace Ineditta.API.Builders.Worksheets
{
    public static class WorksheetStyleBuilder
    {
        public static void SetDefaultStyles(this IXLWorksheet ws)
        {
            ws.SetDefaultCellStyle();
        }
        public static void SetDefaultStyles(this IXLWorksheet ws, int width)
        {
            ws.ColumnWidth = width;

            ws.SetDefaultCellStyle();
        }
        public static void SetDefaultStyles(this IXLWorksheet ws, int width, int height)
        {
            ws.ColumnWidth = width;
            ws.RowHeight = height;

            ws.SetDefaultCellStyle();
        }

        public static void StyleHeader(this IXLWorksheet ws, int row, int column)
        {
            var cell = ws.Cell(row, column);
            cell.Style.Font.Bold = true;
            cell.Style.Fill.SetBackgroundColor(XLColor.FromArgb(4, 128, 190));
            cell.Style.Font.FontColor = XLColor.White;
        }

        public static void StyleHeader(this IXLCell cell)
        {
            cell.Style.Font.Bold = true;
            cell.Style.Fill.SetBackgroundColor(XLColor.FromArgb(4, 128, 190));
            cell.Style.Font.FontColor = XLColor.White;
        }

        private static void SetDefaultCellStyle(this IXLWorksheet ws)
        {
            ws.Rows().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Rows().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Rows().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Rows().Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            ws.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Row(1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Rows().Style.Alignment.WrapText = true;
        }
    }
}
