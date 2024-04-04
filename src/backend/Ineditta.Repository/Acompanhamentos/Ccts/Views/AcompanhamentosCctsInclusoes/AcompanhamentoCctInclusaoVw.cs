namespace Ineditta.Repository.Acompanhamentos.Ccts.Views.AcompanhamentosCctsInclusoes
{
    public class AcompanhamentoCctInclusaoVw
    {
        public long Id { get; set; }
        public DateOnly DataInicial { get; set; }
        public DateOnly? DataFinal { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string? Status { get; set; }
        public string? NomeUsuario { get; set; }
        public string? Fase { get; set; }
        public string NomeDocumento { get; set; } = null!;
        public DateOnly? ProximaLigacao { get; set; }
        public string? DataBase { get; set; }
        public string? SiglaSinditoPatronal { get; set; }
        public string? UfSinditoPatronal { get; set; }
        public string? SiglaSinditoLaboral { get; set; }
        public string? UfSinditoLaboral { get; set; }
        public string? DescricaoSubClasse { get; set; }
        public string? Etiquetas { get; set; }
    }
}
