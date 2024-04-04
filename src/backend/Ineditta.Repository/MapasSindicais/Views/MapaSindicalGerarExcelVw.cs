using Ineditta.Repository.Clausulas.Geral.Models;

namespace Ineditta.Repository.MapasSindicais.Views
{
    public class MapaSindicalGerarExcelVw
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
        public string? DataRegistro { get; set; }
        public DateOnly? DocumentoDataAprovacao { get; set; }
        public DateOnly? DocumentoDataFinal { get; set; }
        public DateOnly? ClausulaGeralDataAprovacao { get; set; }
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
        public string? DocumentoUf { get; set; }
        public string? DocumentoDescricao { get; set; }
        public string? DocumentoTitulo { get; set; }
        public int ClausualaGeralNumero { get; set; }
        public int EstruturaClausulaQuantidadeCamposAdicionais { get; set; }
        public int ClausulaGeralQuantidadeCamposAdicionais { get; set; }
        public int? IdClausulaGeral { get; set; }
        public int? TipoInformacaoId { get; set; }
        public string? TipoInformacaoNome { get; set; }
        public string? TipoDado { get; set; }
        public int? Sequencia { get; set; }
        public string? Texto { get; set; }
        public int? Numerico { get; set; }
        public string? Descricao { get; set; }
        public DateOnly? Data { get; set; }
        public decimal? Percentual { get; set; }
        public string? Hora { get; set; }
        public string? Combo { get; set; }
        public DateOnly? DocumentoValidadeInicicial { get; set; }
        public string? GrupoDados { get; set; }
    }
}


