using Ineditta.Application.Documentos.Sindicais.Dtos;

namespace Ineditta.Repository.Documentos.Sindicais.Views.DocumentosSindicaisVw
{
    public class DocumentoSindicalVw
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public DateTime DataUpload { get; set; }
        public DateOnly DataValidadeInicial { get; set; }
        public DateOnly DataValidadeFinal { get; set; }
        public List<string>? SindicatosLaboraisSiglas { get; set; }
        public List<string>? SindicatosLaboraisMunicipios { get; set; }
        public List<int>? SindicatosLaboraisIds { get; set; }
        public List<int>? SindicatosPatronaisIds { get; set; }
        public List<string>? Assuntos { get; set; }
        public string? AssuntosStringForSearch { get; set; }
        public List<string>? SindicatosPatronaisSiglas { get; set; }
        public string? SindicatosLaboraisSiglasString { get; set; }
        public string? SindicatosPatronaisSiglasString { get; set; }
        public string? Modulo { get; set; }
        public string? DataBase { get; set; }
        public string? Arquivo { get; set; }
        public string? Descricao { get; set; }
        public string? TipoDocumento { get; set; }
        public string? SiglaDoc { get; set; }
        public string? Restrito { get; set; }
        public string? Anuencia { get; set; }
        public DateTime? DataAprovacao { get; set; }
        public int? UsuarioCadastro { get; set; }
        public List<object>? AtividadesEconomicas { get; set; }
        public string? Observacao { get; set; }
        public List<object>? SindicatoPatronal { get; set; }
        public List<object>? SindicatoLaboral { get; set; }
        public List<object>? Abrangencias { get; set; }
        public IEnumerable<Estabelecimento>? Estabelecimentos { get; set; }
        public int? TipoDocId { get; set; }
        public DateOnly? DataLiberacaoClausulas { get; set; }
        public DateOnly? DataInclusao { get; set; }
        public string? AbrangenciaUsuario { get; set; }
    }
}
