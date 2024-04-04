using Ineditta.Application.InformacoesAdicionais.Cliente.Entities;

namespace Ineditta.API.ViewModels.InformacoesAdicionaisClientes.ViewModels
{
    public class InformacaoAdicionalClienteViewModel
    {
        public int GrupoEconomicoId { get; set; }
        public int DocumentoSindicalId { get; set; }
        public IEnumerable<InformacaoAdicional>? InformacoesAdicionais { get; set; }
        public IEnumerable<ObservacaoAdicional>? ObservacoesAdicionais { get; set; }
        public bool Aprovado { get; set; }
        public required string NomeUsuario { get; set; }
        public DateOnly DataUltimaAlteracao { get; set; }
        public string? Orientacao { get; set; }
        public string? OutrasInformacoes { get; set; }
    }
}
