using Ineditta.API.ViewModels.Shared.ViewModels;

namespace Ineditta.API.ViewModels.SindicatosLaborais
{
    public class SindicatoLaboralObterPorIdRequest
    {
        public int Id { get; set; }
        public string? Cnpj { get; set; }
        public required string CodigoSindical { get; set; }
        public required string Sigla { get; set; }
        public string? RazaoSocial { get; set; }
        public string? Denominacao { get; set; }
        public string? Logradouro { get; set; }
        public string? Municipio { get; set; }
        public string? Uf { get; set; }
        public string? Telefone1 { get; set; }
        public string? Telefone2 { get; set; }
        public string? Telefone3 { get; set; }
        public string? Ramal { get; set; }
        public string? Negociador { get; set; }
        public string? Contribuicao { get; set; }
        public string? Enquadramento { get; set; }
        public string? Email1 { get; set; }
        public string? Site { get; set; }
        public OptionModel<int>? Confederacao { get; set; }
        public string? Status { get; set; }
        public string? Email2 { get; set; }
        public string? Email3 { get; set; }
        public string? Twitter { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? Grau { get; set; }
        public OptionModel<int>? Federacao { get; set; }
        public string? Situacao { get; set; }
    }
}
