using Ineditta.Application.ClientesUnidades.Entities;

namespace Ineditta.Application.Documentos.Sindicais.Dtos
{
    public class DocumentoCriadoEmailDto
    {
        public string? OrigemDocumento { get; set; }
        public string? NomeDocumento { get; set; }
        public string? Assunto { get; set; }
        public string? NumeroLegislacao { get; set; }
        public string? FonteLegislacaoSite { get; set; }
        public string? Restrito { get; set; }
        public string? SindicatosLaborais { get; set; }
        public string? SindicatosPatronais { get; set; }
        public DateOnly? ValidadeInicial { get; set; }
        public DateOnly? ValidadeFinal { get; set; }
        public string? Comentarios { get; set; }
        public IEnumerable<ClienteUnidade>? Estabelecimentos { get; set; }
        public string? Abrangencia { get; set; }
        public string? DocumentoLink { get; set; }
        public string? ChamadosLink { get; set; }
        public string? AtividadesEconomicas { get; set; }
    }
}
