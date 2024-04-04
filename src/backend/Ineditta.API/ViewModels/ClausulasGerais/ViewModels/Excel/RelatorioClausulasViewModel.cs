using Ineditta.Application.Documentos.Sindicais.Dtos;

using SindicatoLaboral = Ineditta.Application.Sindicatos.Laborais.Entities.SindicatoLaboral;
using SindicatoPatronal = Ineditta.Application.Sindicatos.Patronais.Entities.SindicatoPatronal;

namespace Ineditta.API.ViewModels.ClausulasGerais.ViewModels.Excel
{
    public class RelatorioClausulasViewModel
    {
        public int IdDocumento { get; set; }
        public string NomeDocumento { get; set; } = null!;
        public DateOnly ValidadeInicial { get; set; }
        public DateOnly? ValidadeFinal { get; set; }
        public IEnumerable<Abrangencia>? Abrangencia { get; set; }
        public IEnumerable<SindicatoLaboral>? SindicatoLaboral { get; set; }
        public IEnumerable<SindicatoPatronal>? SindicatoPatronal { get; set; }
        public long? CodigoDocumentoLegado { get; set; }
        public int IdClausula { get; set; }
        public string NomeClausula { get; set; } = null!;
        public string? GrupoClausula { get; set; }
        public string? Sinonimo { get; set; }
        public string? Assunto { get; set; }
        public DateOnly? DataProcessamento { get; set; }
        public string ResponsavelProcessamento { get; set; } = null!;
        public string? Aprovado { get; set; }
        public int? NumeroClausula { get; set; }
        public string? TextoClausula { get; set; }
    }
}
