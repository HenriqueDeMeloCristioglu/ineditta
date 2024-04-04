namespace Ineditta.API.ViewModels.Comentario.ViewModels
{
    public class ComentarioDataTableViewModel
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public long Id { get; set; }
        public string TipoComentario { get; set; }
        public string TipoUsuarioDestino { get; set; }
        public string TipoNotificacao { get; set; }
        public DateOnly? DataFinal { get; set; }
        public string UsuarioNome { get; set; }
        public string Comentario { get; set; }
        public string EtiquetaNome { get; set; }
        public string SindicatoLaboral { get; set; }
        public string SindicatoPatronal { get; set; }
        public string ClausulaNome { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
