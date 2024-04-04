namespace Ineditta.API.ViewModels.Sindicatos.ViewModels
{
    public class InfoSindicalViewModel
    {
        public string? Sigla { get; set; }
        public string? Cnpj { get; set; }
        public string? RazaoSocial { get; set; }
        public string? Denominacao { get; set; }
        public string? CodigoSindical { get; set; }
        public string? Logradouro { get; set; }
        public string? Municipio { get; set; }
        public string? Uf { get; set; }
        public string? Telefone1 { get; set; }
        public string? Telefone2 { get; set; }
        public string? Telefone3 { get; set; }
        public string? Ramal { get; set; }
        public string? ContatoEnquadramento { get; set; }
        public string? ContatoNegociador { get; set; }
        public string? ContatoContribuicao { get; set; }
        public string? Email1 { get; set; }
        public string? Email2 { get; set; }
        public string? Email3 { get; set; }
        public string? Twitter { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? Site { get; set; }
        public string? DataBase { get; set; }
        public string? AtividadeEconomica { get; set; }
        public AssociacaoSindicatoViewModel? CentralSindical { get; set; }
        public AssociacaoSindicatoViewModel? Federacao { get; set; }
        public AssociacaoSindicatoViewModel? Confederacao { get; set; }
    }
}
