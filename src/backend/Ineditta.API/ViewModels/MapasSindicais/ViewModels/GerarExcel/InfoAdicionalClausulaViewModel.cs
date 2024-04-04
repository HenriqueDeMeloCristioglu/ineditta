using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Attributes;
using Ineditta.Repository.Clausulas.Geral.Models;

namespace Ineditta.API.ViewModels.MapasSindicais.ViewModels.GerarExcel
{
    public class InfoAdicionalClausulaViewModel
    {
        public int? IdClausula { get; set; }
        public string? ClausulaTexto { get; set; }
        public int? IdClausulaGeral { get; set; }
        public int? IdTpInformacao { get; set; }
        public string? NomeTpInformacao { get; set; }
        public string? TipoDado { get; set; }
        public int? Sequencia { get; set; }
        public string? Texto { get; set; }
        public int? Numerico { get; set; }
        public string? Descricao { get; set; }
        public DateOnly? Data { get; set; }
        public decimal? Percentual { get; set; }
        public string? Hora { get; set; }
        public string? Combo { get; set; }
        public IEnumerable<SindLaboral>? SindLaboral { get; set; }
        [NotSearchableDataTable]
        public IEnumerable<SindPatronal>? SindPatronal { get; set; }
        public string? DataRegistro { get; set; }
        public DateOnly? DataAprovacao { get; set; }
        public string? DataBase { get; set; }
        public DateOnly? ValidadeIncial { get; set; }
        public DateOnly? ValidadeFinal { get; set; }
        public string? TituloDocumento { get; set; }
        public string? DescricaoDocumento { get; set; }
        public string? GrupoDados { get; set; }
    }
}