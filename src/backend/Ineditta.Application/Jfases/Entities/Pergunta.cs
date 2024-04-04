namespace Ineditta.Application.Jfases.Entities
{
    public class Pergunta
    {
        public string Tipo { get; set; } = null!;
        public string Texto { get; set; } = null!;
        public IEnumerable<string>? Opcoes { get; set; }
        public IEnumerable<Pergunta>? Adicionais { get; set; }
    }
}