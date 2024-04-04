using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Attributes;
using Ineditta.Repository.Clausulas.Geral.Models;

namespace Ineditta.Repository.Clausulas.Views.ComparativoMapaSindical
{
    public class ComparativoMapaSindicalPrincipalVw
    {
        public int DocumentoId { get; set; }
        [NotSearchableDataTable]
        public SindPatronal[]? SindicatosPatronais { get; set; }
        [NotSearchableDataTable]
        public SindLaboral[]? SindicatosLaborais { get; set; }
        public DateOnly? DataAprovacao { get; set; }
        public string? Database { get; set; }
        [NotSearchableDataTable]
        public CnaeDoc[]? Cnaes { get; set; }
        [NotSearchableDataTable]
        public Abrangencia[]? Abrangencia { get; set; }
        [NotSearchableDataTable]
        public ClienteEstabelecimento[]? Estabelecimentos { get; set; }
        public DateOnly? ValidadeInicial { get; set; }
        public DateOnly? ValidadeFinal { get; set; }
        public string? Uf { get; set; }
        public decimal IndiceProjetado { get; set; }
        public int? InpcId { get; set; }
        public string DocumentoNome { get; set; }
        public DateTime? DataUpload { get; set; }
        public string? Descricao { get; set; }
        public string? SiglasSindicatosPatronais { get; set; }
        public string? SiglasSindicatosLaborais { get; set; }
        public string? CnaesSubclasses { get; set; }
    }
}
