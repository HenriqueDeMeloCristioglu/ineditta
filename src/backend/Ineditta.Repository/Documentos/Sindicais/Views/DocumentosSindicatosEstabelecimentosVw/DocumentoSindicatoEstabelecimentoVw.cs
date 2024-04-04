using Ineditta.Application.Documentos.Sindicais.Dtos;

namespace Ineditta.Repository.Documentos.Sindicais.Views.DocumentosSindicatosEstabelecimentosVw
{
    public class DocumentoSindicatoEstabelecimentoVw
    {
        public int DocumentoId { get; set; }
        public int GrupoEconomicoId { get; set; }
        public string GrupoEconomicoNome { get; set; }
        public int MatrizId { get; set; }
        public string MatrizNome { get; set; }
        public int EstabelecimentoId { get; set; }
        public string EstabelecimentoNome { get; set; }
        public string? EstabelecimentoCodigoSindicatoLaboral { get; set; }
        public string? EstabelecimentoCodigoSindicatoPatronal { get; set; }
        public string? DocumentoTitulo { get; set; }
        public string DocumentoNome { get; set; }
        public string? DocumentoVersao { get; set; }
        public string? DocumentoDatabase { get; set; }
        public DateOnly DocumentoVigenciaInicial { get; set; }
        public DateOnly DocumentoVigenciaFinal { get; set; }
        public string GrupoEconomicoLogo { get; set; }
        public string? EstabelecimentoCodigo { get; set; }
        public DateOnly? DocumentoDataAssinatura { get; set; }
        public string? DocumentoCaminhoArquivo { get; set; }
        public DateOnly? DocumentoDataRegistroMte { get; set; }
        public IEnumerable<SindicatoLaboral>? DocumentoSindicatosLaborais { get; set; }
        public IEnumerable<SindicatoPatronal>? DocumentoSindicatosPatronais { get; set; }
        public IEnumerable<EstabelecimentoSindicatoViewModel>? EstabelecimentoSindicatosLaborais { get; set; }
        public IEnumerable<EstabelecimentoSindicatoViewModel>? EstabelecimentoSindicatosPatronais { get; set; }
    }

    public class EstabelecimentoSindicatoViewModel
    {
        private EstabelecimentoSindicatoViewModel()
        {
        }

        public EstabelecimentoSindicatoViewModel(int id, string cnpj, string sigla, string codigo, string razaoSocial)
        {
            Id = id;
            Cnpj = cnpj;
            Sigla = sigla;
            Codigo = codigo;
            RazaoSocial = razaoSocial;
        }

        public int Id { get; set; }
        public string Cnpj { get; set; }
        public string Sigla { get; set; }
        public string Codigo { get; set; }
        public string RazaoSocial { get; set; }
    }
}
