using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.DocumentosSindicais.Requests
{
    public class ConsultaDocumentoSindicalRequest : DataTableRequest
    {
        public string TipoConsulta { get; set; } = null!;
        public IEnumerable<int>? UnidadesIds { get; set; }
        public IEnumerable<int>? MatrizesIds { get; set; }
        public IEnumerable<int>? GruposEconomicosIds { get; set; }
        public IEnumerable<string>? DatasBases { get; set; }
        public IEnumerable<string>? TiposDocumentos { get; set; }
        public IEnumerable<int>? DocumentosIds { get; set; }
        public DateOnly? DataValidadeInicial { get; set; }
        public DateOnly? DataValidadeFinal { get; set; }
        public IEnumerable<int>? MunicipiosIds { get; set; }
        public IEnumerable<string>? Ufs { get; set; }
        public IEnumerable<int>? CnaesIds { get; set; }
        public IEnumerable<int>? SindicatosPatronaisIds { get; set; }
        public IEnumerable<int>? SindicatosLaboraisIds { get; set; }
        public IEnumerable<int>? AssuntosIds { get; set; }
        public bool Restrito { get; set; }
        public bool AnuenciaObrigatoria { get; set; }
        public IEnumerable<string>? NomesDocumentos { get; set; }
        public IEnumerable<string>? EstruturaClausulasIds { get; set; }
        public IEnumerable<int>? GrupoClausulaIds { get; set; }
        public bool? ShortTemplate { get; set; }
        public bool? ClausulaAprovada { get; set; }
        public bool? Processados { get; set; }
        public bool? Liberados { get; set; }
        public DateOnly? DataAprovacaoInicial { get; set; }
        public DateOnly? DataAprovacaoFinal { get; set; }
        public bool UsarComparacaoEstritaNasValidades { get; set; }
        public int? TipoDocumentoId { get; set; }
        public bool InformacoesAdicionais { get; set; }
    }
}
