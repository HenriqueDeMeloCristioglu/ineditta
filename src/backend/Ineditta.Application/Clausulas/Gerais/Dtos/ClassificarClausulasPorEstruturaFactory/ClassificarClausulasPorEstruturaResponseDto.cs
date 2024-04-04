namespace Ineditta.Application.Clausulas.Gerais.Dtos.ClassificarClausulasPorEstruturaFactory
{
    public class ClassificarClausulasPorEstruturaResponseDto
    {
        public int Id { get; set; }
        public string Texto { get; set; } = null!;
        public string InstrucaoIa { get; set; } = null!;
        public int MaximoPalavrasIa { get; set; }
    }
}
