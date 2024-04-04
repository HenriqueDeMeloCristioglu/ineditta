using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineditta.Repository.Dirigentes.Laborais.Views
{
    public class DirigenteLaboralVw
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Sigla { get; set; }
        public string? Situacao { get; set; }
        public int? Grupo { get; set; }
        public string? Cargo { get; set; }
        public string? RazaoSocial { get; set; }
        public DateOnly? InicioMandato { get; set; }
        public DateOnly? TerminoMandato { get; set; }
        public string? NomeUnidade { get; set; }
        public int? UnidadeId { get; set; }
        public int? SindeId { get; set; }
    }
}
