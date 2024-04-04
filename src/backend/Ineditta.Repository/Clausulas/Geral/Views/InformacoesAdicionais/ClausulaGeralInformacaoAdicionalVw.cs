using Ineditta.Application.Clausulas.Entities.InformacoesAdicionais;
using Ineditta.Repository.Clausulas.Geral.Models;

namespace Ineditta.Repository.Clausulas.Views.InformacoesAdicionais
{
    public class ClausulaGeralInformacaoAdicionalVw
    {
        public int ClausulaId { get; set; }
        public int ClausulaGeralEstruturaId { get; set; }
        public int DocumentoSindicalId { get; set; }
        public int GrupoClausulaId { get; set; }
        public string GrupoClausulaNome { get; set; } = null!;
        public int InformacaoAdicionalId { get; set; }
        public string InformacaoAdicionalNome { get; set; } = null!;
        public string? ValorData { get; set; }
        public decimal? ValorNumerico { get; set; }
        public string? ValorTexto { get; set; }
        public decimal? ValorPercentual { get; set; }
        public string? ValorDescricao { get; set; }
        public string? ValorHora { get; set; }
        public string? ValorCombo { get; set; } 
        public int Sequencia { get; set; }
        public int GrupoDados { get; set; }
        public string DocumentoTitulo { get; set; } = null!;
        public DateOnly? DocumentoDataAprovacao { get; set; }
        public SindPatronal[]? DocumentoSindicatosPatronais { get; set; }
        public SindLaboral[]? DocumentoSindicatosLaborais { get; set; }
        public CnaeDoc[]? AtividadeEconomicas { get; set; }
        public DateOnly DocumentoValidadeInicial { get; set; }
        public DateOnly DocumentoValidadeFinal { get; set; }
        public string DataBase { get; set; } = null!;
        public string EstruturaClausulaTipo { get; set; } = null!;
        public TipoDado InformacaoAdicionalTipoDado { get; set; }
        public bool ClausulaAprovada { get; set; }
        public DateOnly? ClausulaDataAprovacao { get; set; }
        public string? ClausulaTexto { get; set; }
        public bool ClausulaLiberada { get; set; }
        public string? DocumentoUf { get; set; }
        public string TipoDocumentoNome { get; set; } = null!;
        public Abrangencia[]? Abrangencias { get; set; }
        public int EstruturaClausulaId { get; set; }
        public string? CodigosUnidades { get; set; }
        public string? CnpjUnidades { get; set; }
        public string? CodigosSindicatoClienteUnidades { get; set; }
        public string? DenominacoesPatronais { get; set; }
        public string? DenominacoesLaborais { get; set; }
        public string? UfsUnidades { get; set; }
        public string? MunicipiosUnidades { get; set; }
    }
}   
