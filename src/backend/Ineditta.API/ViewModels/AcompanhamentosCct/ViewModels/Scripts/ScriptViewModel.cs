namespace Ineditta.API.ViewModels.AcompanhamentosCct.ViewModels.Scripts
{
    public class ScriptViewModel
    {
        public required string Fase { get; set; }
        public IEnumerable<string>? Respostas { get; set; }
    }
}
