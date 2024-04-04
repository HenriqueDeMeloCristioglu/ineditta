namespace Ineditta.API.ViewModels.EventosCalendario.ViewModels
{
    public class CalendarioSindicalUsuarioInputModel
    {
        public string Titulo { get; set; } = null!;
        public DateTime DataHora { get; set; }
        public TimeSpan? Recorrencia { get; set; }
        public string Local { get; set; } = null!;
        public string? Comentarios { get; set; } = null!;
        public bool Visivel { get; set; }   
    }
}
