using CSharpFunctionalExtensions;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using Ineditta.BuildingBlocks.Core.Files.Pdfs.Dtos;
using System.Text.RegularExpressions;
using System.Text;

namespace Ineditta.BuildingBlocks.Core.Files.Pdfs
{
    public static class PdfManager
    {
        public static Result<bool, Error> IsPdfText(byte[] pdfBytes, CancellationToken cancellationToken = default)
        {
            try
            {
                using MemoryStream stream = new MemoryStream(pdfBytes);
                using PdfReader pdfReader = new PdfReader(stream);
                using PdfDocument pdfDocument = new PdfDocument(pdfReader);
                var maxPages = pdfDocument.GetNumberOfPages() > 1 ? pdfDocument.GetNumberOfPages() - 1 : pdfDocument.GetNumberOfPages();
                for (int pageNum = 1; pageNum <= maxPages; pageNum++)
                {
                    LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();
                    string text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(pageNum), strategy);

                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        return Result.Success<bool, Error>(true);
                    }
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<bool, Error>(Errors.General.Business($"Erro ao processar o PDF: {ex.Message}"));
            }

            return Result.Success<bool, Error>(false);
        }

        public static Result<TextFromPdfDto, Error> ExtractTextFromPdf(byte[] pdfBytes, CancellationToken cancellationToken = default)
        {
            var textFromPdfDto = new TextFromPdfDto();
            var pageDtos = new List<PageFromPdfDto>();

            try
            {
                using MemoryStream stream = new MemoryStream(pdfBytes);
                using PdfReader pdfReader = new PdfReader(stream);
                using PdfDocument pdfDocument = new PdfDocument(pdfReader);
                
                textFromPdfDto.NumberOfPages = pdfDocument.GetNumberOfPages();

                for (int pageNum = 1; pageNum <= textFromPdfDto.NumberOfPages; pageNum++)
                {
                    LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();
                    string text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(pageNum), strategy);

                    pageDtos.Add(new PageFromPdfDto
                    {
                        PageNumber = pageNum,
                        Text = text
                    });
                }
                
                textFromPdfDto.Pages = pageDtos;
                textFromPdfDto.NumberOfPages = pdfDocument.GetNumberOfPages();

                return Result.Success<TextFromPdfDto, Error>(textFromPdfDto);
            }
            catch (Exception ex)
            {
                return Result.Failure<TextFromPdfDto, Error>(Errors.General.Business($"Erro ao extrair texto do PDF: {ex.Message}"));
            }
        }
    }
}
