using Ineditta.Application.ClientesUnidades.Entities;

namespace Ineditta.API.ViewModels.ClientesUnidades.ViewModels
{
    public class ClienteUnidadeViewModel
    {
        public int Id { get; set; }
        public string? Filial { get; set; }
        public string? Nome { get; set; }
        public DateOnly? DataInativacao { get; set; }
        public DateTime? DataAtivacao { get; set; }
        public string? Municipio { get; set; }
        public string? Uf { get; set; }
        public string? Cep { get; set; }
        public string? Bairro { get; set; }
        public string? Regiao { get; set; }
        public string? Logradouro { get; set; }
        public int? LocalizacaoId { get; set; }
        public int? TipoNegocioId { get; set; }
        public string? TipoFilial { get; set; }
        public string? Matriz { get; set; }
        public int? MatrizId { get; set; }
        public string? Grupo { get; set; }
        public string? Cnpj { get; set; }
        public string? NomeGrupoEconomico { get; set; }
        public string? Codigo { get; set; }
        public string? CodigoSindicatoCliente { get; set; }
        public string? CodigoSindicatoPatronal { get; set; }
        public int? CnaeFilialId { get; set; }
        public IEnumerable<CnaeUnidade>? CnaesUnidade { get; set; }
    }
}