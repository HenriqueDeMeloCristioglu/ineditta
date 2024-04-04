using Ineditta.Application.Clausulas.Gerais.Entities;

namespace Ineditta.Repository.Clausulas.Geral.Views.ClausulaGeral
{
    public class ClausulaGeralVw
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public int Processados { get; set; }
        public int NaoProcessados { get; set; }
        public int Aprovados { get; set; }
        public int NaoAprovados { get; set; }
        public DateOnly? DataScrap { get; set; }
        public DateOnly? DataSla { get; set; }
        public DateOnly? DataAprovacao { get; set; }
    }
}
