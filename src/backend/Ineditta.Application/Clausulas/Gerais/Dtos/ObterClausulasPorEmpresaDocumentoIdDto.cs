namespace Ineditta.Application.Clausulas.Gerais.Dtos
{
    public class ObterClausulasPorEmpresaDocumentoIdDto
    {
        public int Id { get; set; }
        public string Texto { get; set; } = null!;
        public int EstruturaClausulaId { get; set; }
        public string InstrucaoIa { get; set; } = null!;
        public int MaximoPalavrasIa { get; set; }
    }
}
