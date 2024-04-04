namespace Ineditta.Application.Clausulas.Gerais.Dtos.ClassificarClausulasPorEstruturaFactory
{
    public class ClassificarClausulasPorEstruturaRequestDto
    {
        public int Id { get; set; }
        public int EstruturaClausulaId { get; set; }
        public string EstruturaClausulaNome { get; set; } = null!;
        public string Texto { get; set; } = null!;
    }
}
