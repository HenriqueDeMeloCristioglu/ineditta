using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Attributes;

namespace Ineditta.API.ViewModels.DiretoriasEmpregados.ViewModels
{
    public class DiretoriaEmpregadoViewModel
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public DateOnly InicioMandato { get; set; }
        public DateOnly? TerminoMandato { get; set; }
        public string? Funcao { get; set; }
        public string? Situacao { get; set; }
        public string? Sigla { get; set; }
        public string? NomeUnidade { get; set; }
        [NotSearchableDataTable]
        public int SindicatoDirigenteId { get; set; }
        [NotSearchableDataTable]
        public string? SindicatoDirigenteSigla { get; set; }

        [NotSearchableDataTable]
        public int? EmpresaId { get; set; }
        [NotSearchableDataTable]
        public string? EmpresaFilial { get; set; }
    }
}
