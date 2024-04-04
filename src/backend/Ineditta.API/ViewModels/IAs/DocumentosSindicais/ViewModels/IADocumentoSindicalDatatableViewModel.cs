namespace Ineditta.API.ViewModels.IAs.DocumentosSindicais.ViewModels
{
    public class IADocumentoSindicalDatatableViewModel
    {
        public long Id { get; set; }
        public int DocumentoReferenciaId { get; set; }
        public string Nome { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string Versao { get; set; } = null!;
        public DateOnly? DataAprovacao { get; set; }
        public DateOnly? DataSla { get; set; }
        public int QuantidadeClausulas { get; set; }
        public string? UsuarioAprovador { get; set; }
        public string StatusGeral { get; set; } = null!;
        public string Assunto { get; set; } = null!;
    }
}
