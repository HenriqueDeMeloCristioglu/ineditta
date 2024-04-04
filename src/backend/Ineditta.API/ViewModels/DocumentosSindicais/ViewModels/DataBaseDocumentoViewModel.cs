using System.Globalization;

using Ineditta.BuildingBlocks.Core.Extensions;

namespace Ineditta.API.ViewModels.DocumentosSindicais.ViewModels
{
    public class DataBaseDocumentoViewModel
    {
        public string? MesAno { get; set; }
#pragma warning disable S3358 // Ternary operators should not be nested
        public int? Ano => string.IsNullOrEmpty(MesAno) || MesAno.Length < 5 ? 0 : (int.TryParse(MesAno?[5..], out var ano) ? ano : null);
        public int Mes => string.IsNullOrEmpty(MesAno) ? 0 : MesAno[0..3].GetMonthNumberByAbbreviatedMonthName();
#pragma warning restore S3358 // Ternary operators should not be nested
    }
}
