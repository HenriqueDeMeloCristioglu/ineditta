
namespace Ineditta.Repository.Comentarios.Views.Comentarios
{
    public class ComentariosVw
    {
        public long Id { get; set; }
        public string Tipo { get; set; } = null!;
        public string TipoUsuarioDestino { get; set; } = null!;
        public string TipoNotificacao { get; set; } = null!;
        public DateOnly? DataValidade { get; set; }
        public string NomeUsuario { get; set; } = null!;
        public long UsuarioId { get; set; }
        public int EtiquetaId { get; set; }
        public string EtiquetaNome { get; set; } = null!;
        public string Comentario { get; set; } = null!;
        public bool Visivel { get; set; }
        public string? SiglaSindicatoPatronal { get; set; } = null!;
        public string? SiglaSindicatoLaboral { get; set; } = null!;
        public string? NomeClausula { get; set; } = null!;
    }
}
