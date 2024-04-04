using Ineditta.Repository.Clausulas.Geral.Models;

namespace Ineditta.Repository.Clausulas.Geral.Views.Clausula
{
    public class ClausulaVw
    {
        public int? ClausulaId { get; set; }
        public string? ClausulaTexto { get; set; }
        public int? GrupoClausulaId { get; set; }
        public string? GrupoClausulaNome { get; set; }
        public int? EstruturaClausulaId { get; set; }
        public string? EstruturaClausulaNome { get; set; }
        public int? DocumentoId { get; set; }
        public SindPatronal[]? DocumentoSindicatoPatronal { get; set; }
        public SindLaboral[]? DocumentoSindicatoLaboral { get; set; }
        public DateOnly? DataRegistro { get; set; }
        public DateOnly? DataAprovacaoDocumento { get; set; }
        public DateOnly? DataAprovacaoClausula { get; set; }
        public string? DocumentoDatabase { get; set; }
        public int? DocumentoTipoId { get; set; }
        public CnaeDoc[]? DocumentoCnae { get; set; }
        public Abrangencia[]? DocumentoAbrangencia { get; set; }
        public ClienteEstabelecimento[]? DocumentoEstabelecimento { get; set; }
        public DateOnly? DocumentoValidadeInicial { get; set; }
        public DateOnly? DocumentoValidadeFinal { get; set; }
        public string? ClausulaGeralLiberada { get; set; }
        public int QuantidadeSindicatosPatronais { get; set; }
        public int QuantidadeSindicatosLaborais { get; set; }
        public string? Aprovado { get; set; }
        public string? DocumentoNome { get; set; }
        public string[]? DocumentoReferencia { get; set; }
        public DateOnly? DataProcessamentoDocumento { get; set; }
        public DateOnly? DataAssinaturaDocumento { get; set; }
        public string? TextoResumido { get; set; }
        public bool ConstaNoDocumento { get; set; }
        public bool Resumivel { get; set; }
        public string? Regiao { get; set; }
        public string? ResumoStatus { get; set; }
        public int? ResumoStatusId { get; set; }
    }
}
