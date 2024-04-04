using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.Ccts.UseCases.Upsert
{
    public class UpsertAcompanhamentoCctRequest : IRequest<Result>
    {
#pragma warning disable CA1805 // Do not initialize unnecessarily
        public long Id { get; set; } = 0;
#pragma warning restore CA1805 // Do not initialize unnecessarily
        public int TipoDocumentoId { get; set; }
        public int UsuarioResponsavelId { get; set; }
        public DateOnly DataInicial { get; set; }
        public long? StatusId { get; set; }
        public long? FaseId { get; set; }
        public string? DataBase { get; set; }
        public DateOnly? DataFinal { get; set; }
        public DateOnly? ProximaLigacao { get; set; }
        public IEnumerable<string>? CnaesIds { get; set; }
        public IEnumerable<int>? EmpresasIds { get; set; }
        public IEnumerable<int>? GruposEconomicosIds { get; set; }
        public IEnumerable<int>? SindicatosLaboraisIds { get; set; }
        public IEnumerable<int>? SindicatosPatronaisIds { get; set; }
        public IEnumerable<int>? LocalizacoesIds { get; set; }
        public IEnumerable<int>? AssuntosIds { get; set; }
        public IEnumerable<int>? EtiquetasIds { get; set; }
        public string? ObservacoesGerais { get; set; }
        public string? Anotacoes { get; set; }
        public DateOnly? DataProcessamento { get; set; }
        public DateOnly? ValidadeFinal { get; set; }
    }
}
