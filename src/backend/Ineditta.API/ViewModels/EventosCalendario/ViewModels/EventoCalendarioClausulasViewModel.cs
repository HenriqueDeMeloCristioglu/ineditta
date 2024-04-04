namespace Ineditta.API.ViewModels.EventosCalendario.ViewModels
{
    public class EventoCalendarioClausulasViewModel
    {
        public long Id { get; set; }
        public DateOnly Data { get; set; }
        public long? ClausulaGeralId { get; set; }
        public string? NomeDocumento { get; set; }
        public string? AtividadesEconomicas { get; set; }
        public string? SiglasSindicatosLaborais { get; set; }
        public string? SiglasSindicatosPatronais { get; set; }
        public string? SindicatoLaboralId { get; set; }
        public string? SindicatoPatronalId { get; set; }
        public string? NomeEvento { get; set; }
        public string? GrupoClausulas { get; set; }
        public string? NomeClausula { get; set; }
        public string? Origem { get; set; }
    }
}
