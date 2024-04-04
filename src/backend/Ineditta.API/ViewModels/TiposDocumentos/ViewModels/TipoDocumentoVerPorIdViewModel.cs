namespace Ineditta.API.ViewModels.TiposDocumentos.ViewModels
{
    public class TipoDocumentoVerPorIdViewModel
    {
        public int Id { get; set; }
        public required string Tipo { get; set; }
        public required string Nome { get; set; }
        public string? Processado { get; set; }
        public string? Sigla { get; set; }
        public string? Permissao { get; set; }
        public string? Modulo { get; set; }
        public string? ValidadeInicial { get; set; }
        public string? ValidadeFinal { get; set; }
        public string? Origem { get; set; }
        public string? PermitirCompartilhar { get; set; }
        public string? DataBase { get; set; }
        public string? SindicatoLaboral { get; set; }
        public string? SindicatoPatronal { get; set; }
        public string? TipoUnidade { get; set; }
        public string? Abrangencia { get; set; }
        public string? AtividadeEconomica { get; set; }
        public string? Estabelecimento { get; set; }
        public string? Assunto { get; set; }
        public string? Descricao { get; set; }
        public string? Numero { get; set; }
        public string? Fonte { get; set; }
        public string? Versao { get; set; }
    }
}
