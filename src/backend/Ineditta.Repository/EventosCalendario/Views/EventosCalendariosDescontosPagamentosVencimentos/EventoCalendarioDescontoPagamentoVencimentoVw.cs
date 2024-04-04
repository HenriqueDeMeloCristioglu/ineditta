using System.Text.RegularExpressions;

using Ineditta.Repository.Models;

using Microsoft.CodeAnalysis.Operations;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ineditta.Repository.EventosCalendario.Views.EventosCalendariosDescontosPagamentosVencimentos
{
    public class EventoCalendarioDescontoPagamentoVencimentoVw
    {
        public long Id { get; set; }
        public DateOnly Data { get; set; }
        public long? ClausulaGeralId { get; set; }
        public string? NomeDocumento { get; set; }
        public string? AtividadesEconomicas { get; set; }
        public string? SiglasSindicatosLaborais { get; set; }
        public string? SiglasSindicatosPatronais { get; set; }
        public string? SindicatoLaboralId { get; set; }
        public string? SindicatoPatronalId { get; set; }
        public string? NomeEvento { get; set; }
        public string? GrupoClausulas { get; set; }
        public string? NomeClausula { get; set; }
        public string? Origem { get; set; }
    }
}