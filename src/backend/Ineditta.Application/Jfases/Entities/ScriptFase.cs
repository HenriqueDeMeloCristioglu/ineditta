namespace Ineditta.Application.Jfases.Entities
{
    public class ScriptFase
    {
        public string Fase { get; set; } = null!;
        public IEnumerable<Pergunta> Perguntas { get; set; } = null!;
    }
}
