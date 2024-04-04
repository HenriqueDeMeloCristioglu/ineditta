using Ineditta.Application.Documentos.Sindicais.Dtos;
using Ineditta.Repository.Documentos.Sindicais.Views.DocumentosSindicatosEstabelecimentosVw;

namespace Ineditta.API.ViewModels.DocumentosSindicais.ViewModels.Exel
{
    public class DocumentoSindicatoEstabelecimentoViewModel
    {
        public int DocumentoId { get; set; }
        public IEnumerable<int> GrupoEconomicoIds { get; set; } = null!;
        public IEnumerable<string> GrupoEconomicoNomes { get; set; } = null!;
        public IEnumerable<int> MatrizIds { get; set; } = null!;
        public IEnumerable<string> MatrizNomes { get; set; } = null!;
        public IEnumerable<int> EstabelecimentosIds { get; set; } = null!;
        public IEnumerable<string> EstabelecimentosNomes { get; set; } = null!;
        public IEnumerable<string>? EstabelecimentoCodigosSindicatosLaborais { get; set; } = null!;
        public IEnumerable<string>? EstabelecimentoCodigosSindicatosPatronais { get; set; } = null!;
        public string? DocumentoTitulo { get; set; }
        public string DocumentoNome { get; set; } = null!;
        public string? DocumentoVersao { get; set; }
        public string? DocumentoDatabase { get; set; }
        public DateOnly DocumentoVigenciaInicial { get; set; }
        public DateOnly DocumentoVigenciaFinal { get; set; }
        public string GrupoEconomicoLogo { get; set; } = null!;
        public IEnumerable<string>? EstabelecimentoCodigo { get; set; } = null!;
        public DateOnly? DocumentoDataAssinatura { get; set; }
        public string? DocumentoCaminhoArquivo { get; set; }
        public DateOnly? DocumentoDataRegistroMte { get; set; }
        public IEnumerable<SindicatoLaboral>? DocumentoSindicatosLaborais { get; set; }
        public IEnumerable<SindicatoPatronal>? DocumentoSindicatosPatronais { get; set; }
        public IEnumerable<EstabelecimentoSindicatoViewModel>? EstabelecimentoSindicatosLaborais { get; set; }
        public IEnumerable<EstabelecimentoSindicatoViewModel>? EstabelecimentoSindicatosPatronais { get; set; }
    }
}
