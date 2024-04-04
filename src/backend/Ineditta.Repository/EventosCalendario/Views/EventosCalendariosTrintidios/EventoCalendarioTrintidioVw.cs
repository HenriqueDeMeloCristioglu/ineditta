using Ineditta.Application.Documentos.Sindicais.Dtos;
using Ineditta.Repository.Models;

using Microsoft.CodeAnalysis.Operations;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ineditta.Repository.EventosCalendario.Views.EventosCalendariosTrintidios
{
    public class EventoCalendarioTrintidioVw
    {
        public DateOnly Data { get; set; }
        public string? Origem { get; set; }
        public DateOnly? ValidadeInicial { get; set; }
        public DateOnly? ValidadeFinal { get; set; }
        public string? AtividadesEconomicas { get; set; }
        public string? SiglasSindicatosLaborais { get; set; }
        public string? SiglasSindicatosPatronais { get; set; }
        public string? SindicatoLaboralId { get; set; }
        public string? SindicatoPatronalId { get; set; }
        public DateOnly? DataBase { get; set; }
        public string? NomeDocumento { get; set; }
        public int? ChaveReferenciaId { get; set; }
        public IEnumerable<SindicatoLaboral>? SindicatosLaborais { get; set; }
        public IEnumerable<SindicatoPatronal>? SindicatosPatronais { get; set; }
    }
}