using Ineditta.Application.AIs.Clausulas.Entities;

namespace Ineditta.API.ViewModels.IAs.Clausulas
{
    public class IAClausulaViewModel
    {
        public int Id { get; set; }
        public string Texto { get; set; } = null!;
        public long DocumentoSindicalId { get; set; }
        public string DocumentoSindicalNome { get; set; } = null!;
        public int? EstruturaClausulaId { get; set; }
        public string? EstruturaClausulaNome { get; set; } = null!;
        public int Numero { get; set; }
        public int? SinonimoId { get; set; }
        public string? SinonimoNome { get; set; } = null!;
        public string Assunto { get; set; } = null!;
        public IAClausulaStatus Status { get; set; }
        public ClausulaClienteGrupoInformacoesAdicionais? GrupoInformacaoAdicional { get; set; }
    }

    public class ClausulaClienteGrupoInformacoesAdicionais
    {
        public int TipoInformacaoId { get; set; }
        public string NomeTipoInformacao { get; set; } = null!;
        public int EstruturaId { get; set; }
    }
}
