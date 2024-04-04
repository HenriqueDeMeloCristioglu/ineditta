using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Attributes;

namespace Ineditta.Repository.ClientesUnidades.Views
{
    public class InformacoesEstabelecimentosVw
    {
        public int UnidadeId  { get; set; }
        public string? CodigoEstabelecimento { get; set; }
        public string? CodigoSindicatoCliente { get; set; }
        public string NomeEstabelecimento { get; set; } = null!;
        public string CnpjEstabalecimento { get; set; } = null!;
        public string? SindicatosLaboraisSiglas { get; set; }
        public string? SindicatosPatronaisSiglas { get; set; }

        [NotSearchableDataTable]
        public IEnumerable<SindicatoLaboralInformacoesEstabelecimentoDto>? SindicatosLaborais { get; set; }

        [NotSearchableDataTable]
        public IEnumerable<SindicatoPatronalInformacoesEstabelecimentoDto>? SindicatosPatronais { get; set; }

        public string DatasBases { get; set; }

        [NotSearchableDataTable]
        public string EmailUsuario { get; set; }
    }

    public class SindicatoLaboralInformacoesEstabelecimentoDto
    {
        public int Id { get; set; }
        public string Sigla { get; set; }
    }

    public class SindicatoPatronalInformacoesEstabelecimentoDto
    {
        public int Id { get; set; }
        public string Sigla { get; set; }
    }
}
