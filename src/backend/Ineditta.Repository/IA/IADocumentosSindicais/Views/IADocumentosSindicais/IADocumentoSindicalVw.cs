using Ineditta.Application.AIs.DocumentosSindicais.Entities;
using Ineditta.BuildingBlocks.Core.Extensions;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Attributes;

namespace Ineditta.Repository.IA.IADocumentosSindicais.Views.IADocumentosSindicais
{
    public class IADocumentoSindicalVw
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public int? DocumentoReferenciaId { get; set; }
        public IADocumentoStatus StatusId { get; set; }
        [NotSearchableDataTable]
        public string? Status => StatusId.GetDescription();
        public string? Versao { get; set; }
        public DateOnly? DataSla { get; set; }
        public DateOnly? DataAprovacao { get; set; }
        public string? UsuarioAprovador { get; set; }
        public int? QuantidadeClausulas { get; set; }
        public string? StatusGeral { get; set; }
        public string? Assunto { get; set; }
        public string? MotivoErro { get; set; }
    }
}
