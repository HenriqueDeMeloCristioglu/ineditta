namespace Ineditta.API.ViewModels.Clausulas.ViewModels
{
    public class ComentarioClausulaViewModel
    {
        public int? IdClausula { get; set; }
        public string? NomeUsuario { get; set; }
        public DateTime? DataRegistro { get; set; }
        public string? Comentario { get; set; }
        public string? Etiqueta { get; set; }
        public int? GrupoEconomicoId { get; set; }
        public IEnumerable<int>? EstabelecimentosIds { get; set; }
    }
}
