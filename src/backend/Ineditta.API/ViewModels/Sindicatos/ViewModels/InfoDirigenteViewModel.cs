namespace Ineditta.API.ViewModels.Sindicatos.ViewModels
{
    public class InfoDirigenteViewModel
    {
        public string? Nome { get; set; }
        public DateOnly? InicioMandato { get; set; }
        public DateOnly? FimMandato { get; set; }
        public string? Funcao { get; set; }
    }
}
