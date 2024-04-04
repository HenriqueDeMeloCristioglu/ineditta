namespace Ineditta.API.ViewModels.AcompanhamentosCct.ViewModels.Excel
{
    public class RelatorioAcompanhamentoCctViewModel
    {
        public IEnumerable<string>? NomeEstabelecimentos { get; set; }
        public IEnumerable<string>? LocalizacoesEstabelecimentos { get; set; }
        public IEnumerable<string>? CnpjEstabelecimentos { get; set; }
        public IEnumerable<string>? CodigoSindicatosEstabelecimentos { get; set; }
        public IEnumerable<string>? CodigoEmpesa { get; set; }
        public IEnumerable<string>? Empresas { get; set; }
        public IEnumerable<string>? SiglasSindicatosLaborais { get; set; }
        public IEnumerable<string>? CnpjSindicatosLaborais { get; set; }
        public IEnumerable<string>? NomeSindicatosLaborais { get; set; }
        public IEnumerable<string>? SiglasSindicatosPatronais { get; set; }
        public IEnumerable<string>? CnpjSindicatosPatronais { get; set; }
        public IEnumerable<string>? NomeSindicatosPatronais { get; set; }
        public string? AtividadeEnconomica { get; set; }
        public string? NomeDocumento { get; set; }
        public string? DataBase { get; set; }
        public string? PeriodoInpc { get; set; }
        public decimal? InpcReal { get; set; }
        public string? Fase { get; set; }
        public string? Observacoes { get; set; }
        public DateOnly? DataProcessamento { get; set; }
        public IEnumerable<string>? Ufs { get; set; }
        public IEnumerable<string>? Municipios { get; set; }
        public IEnumerable<string>? CodigoEstabelecimento { get; set; } = null!;
    }

    public class EstatabelecimentosRelatorioViewModel
    {
        public IEnumerable<string>? Cnpj { get; set; } = null!;
        public IEnumerable<string>? Nome { get; set; } = null!;
        public IEnumerable<string>? CodigoSindicatoCliente { get; set; } = null!;
    }

    public class SindicatosRelatorioViewModel
    {
        public IEnumerable<string>? Cnpj { get; set; } = null!;
        public IEnumerable<string>? Sigla { get; set; } = null!;
        public IEnumerable<string>? Uf { get; set; } = null!;
        public IEnumerable<string>? Nome { get; set; } = null!;
    }
}
