using Ineditta.Repository.Clausulas.Geral.Models;

namespace Ineditta.Repository.MapasSindicais.Views
{
    public class DocumentoMapaSindicalVw
    {
        public int DocumentoId { get; set; }
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
        public string? DocumentoUf { get; set; }
        public string? DocumentoDescricao { get; set; }
        public string? DocumentoTitulo { get; set; }
        public DateOnly? DocumentoValidadeInicicial { get; set; }
        public string TipoDocumentoNome { get; set; }
        public int QuantidadeClausulas { get; set; }
        public int QuantidadeClausulasLiberadas { get; set; }
        public DateOnly? DataLiberacaoClausulas { get; set; }
    }
}
