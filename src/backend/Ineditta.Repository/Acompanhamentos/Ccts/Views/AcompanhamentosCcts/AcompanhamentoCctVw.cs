namespace Ineditta.Repository.Acompanhamentos.Ccts.Views.AcompanhamentosCcts
{
    public class AcompanhamentoCctVw
    {
        public int Id { get; set; }
        public string? NomeDocumento { get; set; }
        public string? DataBase { get; set; }
        public string? Fase { get; set; }
        public long? FaseId { get; set; }
        public string? ObservacoesGerais { get; set; }
        public string? PeriodoAnterior { get; set; }
        public string? Indicador { get; set; }
        public decimal? DadoReal { get; set; }
        public string? IrPeriodo { get; set; }
        public string? AtividadesEconomicas { get; set; }
        public IEnumerable<string>? IdsCnaes { get; set; }
        public string? SindicatosLaboraisIds { get; set; }
        public string? SindicatosLaboraisUfs { get; set; }
        public string? SindicatosLaboraisCnpjs { get; set; }
        public string? SindicatosLaboraisCodigos { get; set; }
        public string? SindicatosLaboraisSiglas { get; set; }
        public string? SindicatosPatronaisIds { get; set; }
        public string? SindicatosPatronaisUfs { get; set; }
        public string? SindicatosPatronaisCnpjs { get; set; }
        public string? SindicatosPatronaisCodigos { get; set; }
        public string? SindicatosPatronaisSiglas { get; set; }
        public DateOnly? DataProcessamento { get; set; }
    }
}
