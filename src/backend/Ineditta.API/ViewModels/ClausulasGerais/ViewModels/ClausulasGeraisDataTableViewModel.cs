namespace Ineditta.API.ViewModels.ClausulasGerais.ViewModels
{
    public class ClausulasGeraisDataTableViewModel
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public int Processadas { get; set; }
        public int NaoProcessadas { get; set; }
        public int Aprovados { get; set; }
        public int NaoAprovados { get; set; }
        public string? DataScrap { get; set; }
        public DateOnly DataSla { get; set; }
        public string? DiasEmProcessamento { get; set; }
    }
}
