using System.ComponentModel.DataAnnotations;

namespace Ineditta.API.ViewModels.Sindicatos.ViewModels
{
    public class SindicatoPorUFDataTableViewModel
    {   
        public int? Id { get; set; }
        public int? SindId { get; set; }
        public string? Sigla { get; set; }
        public string? Comentario { get; set; }
        public string? Etiqueta { get; set; }
        public string? UsuarioResponsavel { get; set; }
        public DateTime? CriadoEm { get; set; }
        public string? SindTipo { get; set; }
        public string? SindRazaoSocial { get; set; }
        public int? GrupoEconomicoResponsavel { get; set; }
    }
}
