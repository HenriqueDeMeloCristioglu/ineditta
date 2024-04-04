namespace Ineditta.Application.Documentos.Sindicais.Dtos
{
    public class InfoDocumentoCriadoDto
    {
        public string? OrigemDocumento { get; set; }
        public string? NomeDocumento { get; set; }
        public IEnumerable<string>? AssuntosIds { get; set; }
        public string? NumeroLegislacao { get; set; }
        public string? FonteLegislacaoSite { get; set; }
        public bool Restrito { get; set; }
        public IEnumerable<SindicatoLaboral>? SindicatosLaborais { get; set; }
        public IEnumerable<SindicatoPatronal>? SindicatosPatronais { get; set; }
        public DateOnly? ValidadeInicial { get; set; }
        public DateOnly? ValidadeFinal { get; set; }
        public string? Comentarios { get; set; }
        public IEnumerable<Estabelecimento>? Estabelecimentos { get; set; }
        public IEnumerable<Abrangencia>? Abrangencia { get; set; }
        public IEnumerable<Cnae>? AtividadesEconomicas { get; set; }
        public string? Assuntos { get; set; }
    }
}
