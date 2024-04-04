using Ineditta.API.ViewModels.Shared.ViewModels;
using Ineditta.Application.Comentarios.Entities;

namespace Ineditta.API.ViewModels.Comentario.ViewModels
{
    public class ComentarioItemViewModel
    {
        public long Id { get; set; }
        public OptionModel<int> TipoComentario { get; set; } = null!;
        public OptionModel<int> Assunto { get; set; } = null!;
        public TipoUsuarioDestino TipoUsuarioDestino { get; set; }
        public int UsuarioDestinoId { get; set; }
        public string UsuarioDestinoDescricao { get; set; } = null!;
        public TipoNotificacao TipoNotificacaoId { get; set; }
        public DateOnly? DataFinal { get; set; }
        public long AdministradorId { get; set; }
        public string AdministradorNome { get; set; } = null!;
        public string? Comentario { get; set; }
        public ComentarioEtiqueta Etiqueta { get; set; } = null!;
        public bool Visivel { get; set; }
    }

    public class ComentarioEtiqueta
    {
        public long Id { get; set; }
        public string Nome { get; set; } = null!;
        public ComentarioTipoEtiqueta Tipo { get; set; } = null!;
    }

    public class ComentarioTipoEtiqueta
    {
        public long Id { get; set; }
        public string Nome { get; set; } = null!;
    }
}
