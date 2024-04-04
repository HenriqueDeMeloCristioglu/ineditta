namespace Ineditta.BuildingBlocks.Core.Files.Pdfs.Dtos
{
    public class TextFromPdfDto
    {
        public int NumberOfPages { get; set; }
        public IEnumerable<PageFromPdfDto> Pages { get; set; } = null!;
        public string ConcatenatedText => string.Join(" ", Pages.Select(page => page.Text));
    }

    public class PageFromPdfDto
    {
        public int PageNumber { get; set; }
        public string Text { get; set; } = null!;
    }
}
