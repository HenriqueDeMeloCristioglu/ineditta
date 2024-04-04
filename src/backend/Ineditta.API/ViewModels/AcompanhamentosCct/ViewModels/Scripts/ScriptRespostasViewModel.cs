using Ineditta.Application.AcompanhamentosCcts.Entities;

namespace Ineditta.API.ViewModels.AcompanhamentosCct.ViewModels.Scripts
{
    public class ScriptRespostasViewModel
    {
        public string Fase { get; set; } = null!;
        public string? Horario { get; set; }
        public string? NomeUsuario { get; set; }
        public string? Respostas { get; set; }
        public string? Perguntas { get; set; }
    }
}
