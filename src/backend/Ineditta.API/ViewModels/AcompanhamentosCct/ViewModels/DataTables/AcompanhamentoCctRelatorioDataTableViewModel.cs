namespace Ineditta.API.ViewModels.AcompanhamentosCct.ViewModels.DataTables
{
    public class AcompanhamentoCctRelatorioDataTableViewModel
    {
        public string? SiglasSindicatosLaborais { get; set; }
        public string? CnpjsLaborais { get; set; }
        public string? UfsSindicatoLaborais { get; set; }
        public string? SiglasSindicatoPatronais { get; set; }
        public string? CnpjsPatronais { get; set; }
        public string? UfsSindicatoPatronais { get; set; }
        public string? AtividadeEconomica { get; set; }
        public string? NomeDocumento { get; set; }
        public string? DataBase { get; set; }
        public string? PeriodoInpc { get; set; }
        public decimal? InpcReal { get; set; }
        public string? Fase { get; set; }
        public string? Observacoes { get; set; }
        public DateOnly? DataProcessamento { get; set; }
    }
}
