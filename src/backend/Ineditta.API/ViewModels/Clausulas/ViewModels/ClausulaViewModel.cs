using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Attributes;
using Ineditta.Repository.Clausulas.Geral.Models;

namespace Ineditta.API.ViewModels.Clausulas.ViewModels
{
    public class ClausulaViewModel
    {
        public int? Id { get; set; }
        public string? GrupoClausula { get; set; }
        public string? NomeClausula { get; set; }
        [NotSearchableDataTable]
        public IEnumerable<SindLaboral>? SindLaboral { get; set; }
        [NotSearchableDataTable]
        public IEnumerable<SindPatronal>? SindPatronal { get; set; }
        public string? TextoClausula { get; set; }
        public string? DataBase { get; set; }
        public IEnumerable<CnaeDoc>? Cnae { get; set; }
        public DateOnly? ValidadeInicial { get; set; }
        public DateOnly? ValidadeFinal { get; set; }
        public DateOnly? DataAprovacao { get; set; }
        public DateOnly? DataAprovacaoClausula { get; set; }
        public IEnumerable<ClienteEstabelecimento>? Unidade { get; set; }
        public IEnumerable<string>? Referencia { get; set; }
        public string? NomeDocumento { get; set; }
        public IEnumerable<ComentarioClausulaViewModel>? Comentarios { get; set; }
        public bool PossuiInformacaoAdicional { get; set; }
        public DateOnly? DataProcessamentoDocumento { get; set; }
        public string? TextoResumido { get; set; }
        public DateOnly? DataAssinaturaDocumento { get; set; }
        public string? TextoResumidoCliente { get; set; }
        public int DocumentoId { get; set; }
        public string? MunicipioPrimeiroSindicatoLaboral { get; set; }
    }
}
