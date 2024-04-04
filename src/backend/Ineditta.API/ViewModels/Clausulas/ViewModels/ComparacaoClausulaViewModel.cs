using DiffPlex.DiffBuilder.Model;

using Ineditta.API.ViewModels.SindicatosPatronais.ViewModels;

namespace Ineditta.API.ViewModels.Clausulas.ViewModels
{
    public class ComparacaoClausulaViewModel
    {
        public SindicatoPatronalViewModel? SindicatoPatronal { get; set; }
        public IEnumerable<ComparacaoClausulaItem>?  ComparacaoClausulaItems { get; set; }
    }
    public class ComparacaoClausulaItem
    {
        public string? TextoReferencia { get; set; }
        public string? TextoAnterior { get; set; }
        public string? Clausula { get; set; }
        public string? Grupo { get; set; }
        public int? IdAssuntoReferencia { get; set; }
        public int? IdAssuntoAnterior { get; set; }
        public SideBySideDiffModel? Diferenca { get; set; }
        public SideBySideDiffModel? DiferencaTitulo { get; set; }
        public string? TituloClausulaReferencia { get; set; }
        public string? TituloClausulaAnterior { get; set; }
        public string[]? TextoSeparado { get; set; }
    }

    public class ComparacaoClausulaSindPatronal
    {
        public string? Uf { get; set; }
        public int? Id { get; set; }
        public string? Cnpj { get; set; }
        public string? Sigla { get; set; }
        public string? Municipio { get; set; }
    }
}
