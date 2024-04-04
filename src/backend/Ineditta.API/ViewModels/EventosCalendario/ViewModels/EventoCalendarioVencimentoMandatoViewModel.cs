namespace Ineditta.API.ViewModels.EventosCalendario.ViewModels
{
    public class EventoCalendarioVencimentoMandatoViewModel
    {
        public DateOnly Data { get; set; }
        public string? Origem { get; set; }
        public DateOnly? ValidadeInicial { get; set; }
        public DateOnly? ValidadeFinal { get; set; }
        public string? SiglaSindicato { get; set; }
        public int? SindId { get; set; }
    }
}
