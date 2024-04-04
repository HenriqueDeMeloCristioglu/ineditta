namespace Ineditta.Repository.Documentos.Sindicais.Views.DocumentosSindicaisClausulasVw
{
    public class DocumentoSindicalClausulasVw
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public DateTime DataUpload { get; set; }
        public DateOnly DataValidadeInicial { get; set; }
        public DateOnly DataValidadeFinal { get; set; }
        public List<string>? SindicatosLaboraisSiglas { get; set; }
        public List<string>? SindicatosLaboraisMunicipios { get; set; }
        public List<string>? SindicatosPatronaisSiglas { get; set; }
        public string? Modulo { get; set; }
        public string? DataBase { get; set; }
        public string? Arquivo { get; set; }
        public string? Descricao { get; set; }
        public string? TipoDocumento { get; set; }
        public string? SiglaDoc { get; set; }
        public string? Restrito { get; set; }
        public string? Anuencia { get; set; }
        public DateTime? DataAprovacao { get; set; }
        public int ClausulaQuantidadeNaoAprovadas { get; set; }
        public DateTime? ClausulaDataUltimaAprovacao { get; set; }
    }
}
