namespace Ineditta.API.ViewModels.ClausulasClientes.ViewModels
{
    public class ClausulClienteRelatorioBuscaRapidaExcel
    {
        public long Id { get; set; }
        public int ClausulaId { get; set; }
        public int GrupoEconomicoId { get; set; }
        public int EstruturaClausulaId { get; set; }
        public int DocumentoId { get; set; }
        public string Texto { get; set; } = null!;
    }
}
