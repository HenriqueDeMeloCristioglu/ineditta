
namespace Ineditta.Repository.Comentarios.Views
{
    public class ComentarioSindicatoVw
    {
        public string? Tipo { get; set; }
        public string? Comentario { get; set; }
        public int? SindicatoId { get; set; }
        public string? SindicatoSigla { get; set; }
        public string? SindicatoRazaoSocial { get; set; }
        public string? SindicatoTipo { get; set; }
        public string? ComentarioEtiqueta { get; set; }
        public int? ComentarioResponsavelId { get; set; }
        public string? ComentarioResponsavelNome { get; set; }
        public DateTime? ComentarioDataRegistro { get; set; }
        public int? ComentarioResponsavelGrupoEconomicoId { get; set; }
        public string? ComentarioResponsavelNivel { get; set; }
        public string? SindicatoUf { get; set; }
        public string? ComentarioTipoDestino { get; set; }
        public int? ComentarioDestinoId { get; set; }
        public int? ComentarioEstabelecimentoDestinoId { get; set; }
        public int? ComentarioEmpresaDestinoId { get; set; }
        public int? ComentarioEmpresaGrupoEconomicoDestinoId { get; set; }

        public bool Visivel { get; set; }
    }
}
