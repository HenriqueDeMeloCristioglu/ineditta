using Ineditta.Application.Documentos.Sindicais.Dtos;

using Newtonsoft.Json;

namespace Ineditta.API.ViewModels.ClausulasGerais.ViewModels.Excel
{
    public class RelatorioBuscaRapidaExcelViewModel
    {
        public int DocumentoId { get; set; }
        public string? CodigosSindicatoCliente { get; set; }
        public string? CodigosUnidades { get; set; }
        public string? CnpjsUnidades { get; set; }
        public string? UfsUnidades { get; set; }
        public string? MunicipiosUnidades { get; set; }
        public string? SiglasSindicatosLaborais { get; set; }
        public string? CnpjsSindicatosLaborais { get; set; }
        public string? DenominacoesSindicatosLaborais { get; set; }
        public string? SiglasSindicatosPatronais { get; set; }
        public string? CnpjsSindicatosPatronais { get; set; }
        public string? DenominacoesSindicatosPatronais { get; set; }
        public DateOnly? DataLiberacaoClausulas { get; set; }
        public string? NomeDocumento { get; set; }
        public string? AtividadesEconomicasDocumentoString { get; set; }
        public IEnumerable<Cnae>? AtividadesEconomicasDocumento => AtividadesEconomicasDocumentoString is not null ? JsonConvert.DeserializeObject<IEnumerable<Cnae>?>(AtividadesEconomicasDocumentoString) : null;
        public DateOnly? ValidadeInicialDocumento { get; set; }
        public DateOnly? ValidadeFinalDocumento { get; set; }
        public DateOnly? DataAssinaturaDocumento { get; set; }
        public string? DatabaseDocumento { get; set; }
        public string? AbrangenciaDocumentoString { get; set; }
        public IEnumerable<Abrangencia>? AbrangenciaDocumento => AbrangenciaDocumentoString is not null ? JsonConvert.DeserializeObject<IEnumerable<Abrangencia>?>(AbrangenciaDocumentoString) : null;
        public IEnumerable<ClausulaRelatorioBuscaRapidaViewModel> Clausulas { get; set; } = null!;
    }

    public class ClausulaRelatorioBuscaRapidaViewModel
    {
        public int Id { get; set; }
        public int EstruturaClausulaId { get; set; }
        public int DocumentoId { get; set; }
        public string? Nome { get; set; }
        public string? NomeGrupo { get; set; }
        public string? Numero { get; set; }
        public string? Texto { get; set; }
    }
}
