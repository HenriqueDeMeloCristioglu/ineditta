using Ineditta.API.ViewModels.Shared.ViewModels;

namespace Ineditta.API.ViewModels.ClausulasGerais.ViewModels
{
    public class ClausulasGeraisViewModel
    {
        public int Id { get; set; }
        public int IdEstruturaClausula { get; set; }
        public required string Nome { get; set; }
        public required string Texto { get; set; }
        public DateOnly? DataAprovacao { get; set; }
        public int? Aprovador { get; set; }
        public int? Numero { get; set; }
        public required DocumentoViewModel Documento { get; set; }
        public AssuntoViewModel? Assunto { get; set; }
        public OptionModel<int>? Sinonimo { get; set; }
        public string? TextoResumido { get; set; }
    }

    public class AssuntoViewModel
    {
        public int? Id { get; set; }
        public string? Descricao { get; set; }
    }

    public class DocumentoViewModel
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public DateOnly ValidadeInicial { get; set; }
        public DateOnly? ValidadeFinal { get; set; }
    }
}
