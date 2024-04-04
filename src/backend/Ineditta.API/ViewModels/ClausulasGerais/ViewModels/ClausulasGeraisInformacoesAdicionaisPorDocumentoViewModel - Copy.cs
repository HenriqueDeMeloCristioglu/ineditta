namespace Ineditta.API.ViewModels.ClausulasGerais.ViewModels
{
    public class DocumentoSindicalClausulaVw
    {
        public int? Id { get; set; }
        public string? NomeEstruraClausula { get; set; }
        public string? NomeGrupoClausula { get; set; }
        public int? Numero { get; set; }
        public int? EstruturaId { get; set; }
        public int? DocumentoId { get; set; }
        public IEnumerable<Grupos>? Grupos { get; set; }
    }

    public class Grupos
    {
        public int? Id { get; set; }
        public string? Nome { get; set; }
        public IEnumerable<InformacaoAdicionalSisap>? InformacoesAdicionais { get; set; }
    }

    public class InformacaoAdicionalSisap
    {
        public int Id { get; set; }
        public string? Tipo { get; set; }
        public int? Codigo { get; set; }
        public string? Descricao { get; set; }
        public int? SequenciaLinha { get; set; }
        public int? SequenciaItem { get; set; }
        public required InformacaoAdicionalDado Dado { get; set; }
    }

    public class InformacaoAdicionalDado
    {
        public string? Nome { get; set; }
        public string? Texto { get; set; }
        public Combo? Combo { get; set; }
        public string? Hora { get; set; }
        public decimal? Percentual { get; set; }
        public string? Data { get; set; }
        public string? Descricao { get; set; }
        public decimal? Numerico { get; set; }
    }

    public class Combo
    {
        public IEnumerable<string>? Opcoes { get; set; }
        public IEnumerable<string>? Valor { get; set; }
    }
}
