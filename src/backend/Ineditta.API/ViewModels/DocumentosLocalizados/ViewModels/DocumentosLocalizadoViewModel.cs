using Ineditta.Application.ClientesUnidades.Entities;
using Ineditta.Application.Documentos.Sindicais.Dtos;

namespace Ineditta.API.ViewModels.DocumentosLocalizados.ViewModels
{
    public class DocumentosLocalizadoViewModel
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public int IdTipoDoc { get; set; }
        public string? Sigla { get; set; }
        public string? Uf { get; set; }
        public string? VersaoDocumento { get; set; }
        public string? Origem { get; set; }
        public string? NumeroSolicitacaoMR { get; set; }
        public string? NumRegMTE { get; set; }
        public DateOnly ValidadeInicial { get; set; }
        public DateOnly? ValidadeFinal { get; set; }
        public DateOnly? ProrrogacaoDoc { get; set; }
        public DateOnly? DataAssinatura { get; set; }
        public DateOnly? DataRegMTE { get; set; }
        public string? Permissao { get; set; }
        public string? Observacao { get; set; }
        public int? IdTipoNegocio { get; set; }
        public string? NomeDocumento { get; set; }
        public string? Caminho { get; set; }
        public bool? DocRestrito { get; set; }
        public string? Database { get; set; }
        public int? UsuarioResponsavel { get; set; }
        public IEnumerable<Estabelecimento>? ClienteEstabelecimento { get; set; }
        public IEnumerable<string>? Referencia { get; set; }
        public IEnumerable<SindicatoLaboral>? SindLaboral { get; set; }
        public IEnumerable<SindicatoPatronal>? SindPatronal { get; set; }
        public IEnumerable<Cnae>? CnaeDoc { get; set; }
        public IEnumerable<Abrangencia>? Abrangencia { get; set; }
        public long? IdDocumentoLocalizado { get; set; }
        public IEnumerable<ClienteUnidade>? ClienteUnidades { get; set; }
        public DateOnly? DataLiberacaoClausulas { get; set; }
    }
}
