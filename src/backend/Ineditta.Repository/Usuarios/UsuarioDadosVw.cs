namespace Ineditta.Repository.Usuarios
{
    public class UsuarioDadosVw
    {
        public int UsuarioId { get; set; }
        public int? UnidadeId { get; set; }
        public string? UnidadeNome { get; set; }
        public string? UnidadeCnpj { get; set; }
        public string? UnidadeCodigo { get; set; }
        public int? CodigoSindicatoCliente { get; set; }
        public IEnumerable<int>? Cnaes { get; set; }
        public string? SubclasseDescricao { get; set; }
        public int? EmpresaId { get; set; }
        public string? EmpresaCnpj { get; set; }
        public string? EmpresaCodigo { get; set; }
        public string? EmpresaRazaoSocial { get; set; }
        public int? GrupoEconomicoId { get; set; }
        public string? GrupoEconomicoNome { get; set; }
        public int? LocalizacaoId { get; set; }
        public string? Municipio { get; set; }
        public string? Uf { get; set; }
        public string? Regiao { get; set; }
        public IEnumerable<int>? SindicatosLaborais { get; set; }
        public IEnumerable<int>? SindicatosPatronais { get; set; }
        public int? SindicatoLaboralBaseTerritorialId { get; set; }
        public int? SindicatoPatronalBaseTerritorialId { get; set; }
    }
}
