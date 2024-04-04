using Ineditta.Application.ClientesMatriz.Entities;

namespace Ineditta.API.ViewModels.ClientesMatriz.ViewModels
{
    public class ClienteMatrizModulosTipoDocumentoViewModel
    {
        public int Id { get; set; }
        public int? GrupoEconomicoId { get; set; }
        public string? Codigo { get; set; }
        public string? Nome { get; set; }
        public int? AberturaNegociacao { get; set; }
        public string? SlaPrioridade { get; set; }
        public IEnumerable<int>? TiposDocumentosIds { get; set; }
        public TipoProcessamento? TipoProcessamento { get; set; }
        public int? DataCorteFopag { get; set; }
        public DateTime? DataInativacao { get; set; }
        public IEnumerable<int>? ModulosIds { get; set; }
    }
}
