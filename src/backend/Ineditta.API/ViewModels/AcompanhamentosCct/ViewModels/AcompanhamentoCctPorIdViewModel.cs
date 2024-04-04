using System.Text.Json;
using System.Text.Json.Serialization;

using Ineditta.Application.Jfases.Entities;
using Ineditta.BuildingBlocks.Core.Extensions;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Attributes;

namespace Ineditta.API.ViewModels.AcompanhamentosCct.ViewModels
{
    public class AcompanhamentoCctPorIdViewModel
    {
        public int Id { get; set; }
        public DateOnly DataInicial { get; set; }
        public DateOnly? DataFinal { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public long? StatusId { get; set; }
        public string? Status { get; set; }
        public int UsuarioResponsavelId { get; set; }
        public long? FaseId { get; set; }
        public string? Fase { get; set; }
        public string? ObservacoesGerais { get; set; }
        public int TipoDocumentoId { get; set; }
        public string? DataBase { get; set; }
        public IEnumerable<string>? ListaEmailDestinatarioClientes { get; set; }

        [JsonIgnore]
        public string? GruposEconomicosIds { get; set; }
        [NotSearchableDataTable]
        public IEnumerable<int>? GruposEconomicos => string.IsNullOrEmpty(GruposEconomicosIds) ? Enumerable.Empty<int>() : GruposEconomicosIds!.SplitToInt(",");

        [JsonIgnore]
        public string? EmpresasIds { get; set; }
        [NotSearchableDataTable]
        public IEnumerable<int>? Empresas => string.IsNullOrEmpty(EmpresasIds) ? Enumerable.Empty<int>() : EmpresasIds!.SplitToInt(",");

        public string? Anotacoes { get; set; }
        public DateOnly? DataProcessamento { get; set; }
        public string NomeUsuario { get; set; } = null!;
        public string? NomeTipoDocumento { get; set; }
        public int? IdTipoDocumento { get; set; }
        public string? AtividadesEconomicas { get; set; }
        public string? JForm { get; set; }
        public DateOnly? ValidadeFinal { get; set; }

        [JsonIgnore]
        public string? LocalizacoesIdsJson { get; set; }
        [NotSearchableDataTable]
        public IEnumerable<int>? LocalizacoesIds => string.IsNullOrEmpty(LocalizacoesIdsJson) ? Enumerable.Empty<int>() :
            JsonSerializer.Deserialize<IEnumerable<int>>(LocalizacoesIdsJson);

        [JsonIgnore]
        public string? CnaesIds { get; set; }
        [NotSearchableDataTable]
        public IEnumerable<string>? Cnaes => string.IsNullOrEmpty(CnaesIds) ? Enumerable.Empty<string>() :
            JsonSerializer.Deserialize<IEnumerable<string>>(CnaesIds);

        [JsonIgnore]
        public string? SindicatosLaboraisJson { get; set; }
        [NotSearchableDataTable]
        public IEnumerable<SindicatoAdicionalViewModel>? SindicatosLaborais => string.IsNullOrEmpty(SindicatosLaboraisJson) ? Enumerable.Empty<SindicatoAdicionalViewModel>() :
            JsonSerializer.Deserialize<IEnumerable<SindicatoAdicionalViewModel>>(SindicatosLaboraisJson);

        [JsonIgnore]
        public string? SindicatosPatronaisJson { get; set; }
        [NotSearchableDataTable]
        public IEnumerable<SindicatoAdicionalViewModel>? SindicatosPatronais => string.IsNullOrEmpty(SindicatosPatronaisJson) ? Enumerable.Empty<SindicatoAdicionalViewModel>() :
            JsonSerializer.Deserialize<IEnumerable<SindicatoAdicionalViewModel>>(SindicatosPatronaisJson);

        [JsonIgnore]
        public string? AssuntosJson { get; set; }
        [NotSearchableDataTable]
        public IEnumerable<AcompanhamentoCctAssuntoViewModel>? Assuntos => string.IsNullOrEmpty(AssuntosJson) ? Enumerable.Empty<AcompanhamentoCctAssuntoViewModel>() :
            JsonSerializer.Deserialize<IEnumerable<AcompanhamentoCctAssuntoViewModel>>(AssuntosJson);

        [JsonIgnore]
        public string? EtiquetasJson { get; set; }
        [NotSearchableDataTable]
        public IEnumerable<AcompanhamentoCctEtiquetaViewModel>? Etiquetas => string.IsNullOrEmpty(EtiquetasJson) ? Enumerable.Empty<AcompanhamentoCctEtiquetaViewModel>() :
            JsonSerializer.Deserialize<IEnumerable<AcompanhamentoCctEtiquetaViewModel>>(EtiquetasJson);
    }
}
