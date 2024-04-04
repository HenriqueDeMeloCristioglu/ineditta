namespace Ineditta.Repository.AcompanhamentosCct.Views.AcompanhamentosCctsRelatorios
{
    public class AcompanhamentoCctRelatorioVw
    {
        public int Id { get; set; }
        public string? AtividadesEconomicas { get; set; }
        public string? NomeDocumento { get; set; }
        public string? DataBase { get; set; }
        public string? PeriodoAnterior { get; set; }
        public string? Indicador { get; set; }
        public decimal? DadoReal { get; set; }
        public string? Fase { get; set; }
        public long? FaseId { get; set; }
        public string? ObservacoesGerais { get; set; }
        public DateOnly? DataProcessamento { get; set; }
        public string? IrPeriodo { get; set; }
        public IEnumerable<EstatabelecimentoRelatorio>? Estabelecimentos { get; set; }
        public IEnumerable<SindicatoRelatorio>? SindicatosLaborais { get; set; }
        public IEnumerable<SindicatoRelatorio>? SindicatosPatronais { get; set; }
        public IEnumerable<string>? Ufs { get; set; }
        public IEnumerable<string>? Municipios { get; set; }
    }

    public class EstatabelecimentoRelatorio
    {
        public string? Cnpj { get; set; }
        public string? Nome { get; set; }
        public string? GrupoEconomicoId { get; set; }
        public string? CodigoSindicatoCliente { get; set; }
        public string? CodigoEstabelecimento { get; set; }
        public string? CodigoEmpresa { get; set; }
        public string? NomeEmpresa { get; set; }
        public string? LocalizacaoEstabelecimento { get; set; }
    }

    public class SindicatoRelatorio
    {
        public string? Cnpj { get; set; }
        public string? Sigla { get; set; }
        public string? Uf { get; set; }
        public string? Nome { get; set; }
    }
}
