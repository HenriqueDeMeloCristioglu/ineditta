using System.Text.Json;
using System.Text.Json.Serialization;

using Ineditta.BuildingBlocks.Core.Extensions;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Attributes;

namespace Ineditta.API.ViewModels.AcompanhamentosCct.ViewModels
{
    public class AcompanhamentoCctViewModel
    {
        public int Id { get; set; }
        public DateOnly DataInicial { get; set; }
        public DateOnly? DataFinal { get; set; }
        public DateOnly? UltimaAtualizacao { get; set; }
        public string? Status { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public string? Fase { get; set; }
        public string NomeDocumento { get; set; } = null!;
        public DateOnly? ProxLigacao { get; set; }
        public string? DataBase { get; set; }
        public string SiglaSindPatronal { get; set; } = null!;
        public string? SindicatoPatronalEmail { get; set; }
        public string? UfSindPatronal { get; set; }
        public string? MunicipioSindPatronal { get; set; }
        public string SiglaSindEmpregado { get; set; } = null!;
        public string? SindicatoLaboralEmail { get; set; }
        public string? UfSindEmpregado { get; set; }
        public string DescricaoSubClasse { get; set; } = null!;
        public string? ObservacoesGerais { get; set; }
        public int IdSindPatronal { get; set; }
        public int IdSindLaboral { get; set; }
        public string? IdsSindPatronalAdicionais { get; set; }
        public string? IdsSindLaboralAdicionais { get; set; }
        public int IdUsuarioAdm { get; set; }
        public int IdTipoDoc { get; set; }
        public string? NomeTipoDoc { get; set; }
        public string? MunicipioSindLaboral { get; set; }
        public string? ComentarioLaboral { get; set; }
        public string? ComentarioPatronal { get; set; }
        public string? IdsCnaes { get; set; }
        public int TipoDoc { get; set; }
        public string Atvs { get; set; } = null!;
        public string? CnaePatronal { get; set; }
        public string? CnaeLaboral { get; set; }
        public string? CnpjPatronal { get; set; }
        public string? PeriodoInpc { get; set; }
        public string? InpcReal { get; set; }
        public string? Abrangencia { get; set; }
        public string? CnpjLaboral { get; set; }
        [NotSearchableDataTable]
        public IEnumerable<int>? GruposEconomicos => string.IsNullOrEmpty(GruposEconomicosIds) ? Enumerable.Empty<int>() : GruposEconomicosIds!.SplitToInt(",");
        [JsonIgnore]
        public string? GruposEconomicosIds { get; set; }
        [NotSearchableDataTable]
        public IEnumerable<int>? Empresas => string.IsNullOrEmpty(EmpresasIds) ? Enumerable.Empty<int>() : EmpresasIds!.SplitToInt(",");
        [JsonIgnore]
        public string? EmpresasIds { get; set; }

        [JsonIgnore]
        public string? SindicatosLaborais { get; set; }

        [JsonIgnore]
        public string? SindicatosPatronais { get; set; }

        [JsonIgnore]
        public string? CnaesJson { get; set; }

        [NotSearchableDataTable]
        public IEnumerable<SindicatoAdicionalViewModel>? SindicatosPatronaisAdicionais => string.IsNullOrEmpty(SindicatosPatronais) ? Enumerable.Empty<SindicatoAdicionalViewModel>() :
            JsonSerializer.Deserialize<IEnumerable<SindicatoAdicionalViewModel>>(SindicatosPatronais);

        [NotSearchableDataTable]
        public IEnumerable<SindicatoAdicionalViewModel>? SindicatosLaboraisAdicionais => string.IsNullOrEmpty(SindicatosLaborais) ? Enumerable.Empty<SindicatoAdicionalViewModel>() :
            JsonSerializer.Deserialize<IEnumerable<SindicatoAdicionalViewModel>>(SindicatosLaborais);

        [NotSearchableDataTable]
        public IEnumerable<CnaesAcompanhamentoCct>? Cnaes => string.IsNullOrEmpty(CnaesJson) ? Enumerable.Empty<CnaesAcompanhamentoCct>() :
            JsonSerializer.Deserialize<IEnumerable<CnaesAcompanhamentoCct>>(CnaesJson);
    }
}
