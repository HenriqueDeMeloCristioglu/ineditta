namespace Ineditta.Application.SharedKernel.Ocr.Dtos
{
    public class ExtractTextResponseDto
    {
        public ExtractTextResponseDto(string text)
        {
            Text = text;
        }

        public string Text { get; set; }
    }
}
