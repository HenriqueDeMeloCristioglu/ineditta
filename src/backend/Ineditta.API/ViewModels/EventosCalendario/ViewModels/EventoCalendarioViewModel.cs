﻿using Ineditta.Repository.EventosCalendario.Views.EventosCalendariosAssembleiasReunioes;

namespace Ineditta.API.ViewModels.EventosCalendario.ViewModels
{
    public class EventoCalendarioViewModel
    {
        public long? Id { get; set; }
        public DateOnly? Data { get; set; }
        public DateTime? DataHora { get; set; }
        public string? TipoEvento { get; set; }
        public string? NomeDocumento { get; set; }
        public string? Origem { get; set; }
        public DateOnly? ValidadeInicial { get; set; }
        public DateOnly? ValidadeFinal { get; set; }
        public string? AtividadesEconomicas { get; set; }
        public IEnumerable<SindicatoEvento>? SindicatosLaborais { get; set; }
        public IEnumerable<SindicatoEvento>? SindicatosPatronais { get; set; }
        public int? ChaveReferenciaId { get; set; }
        public string? Fase { get; set; }
        public string? DataBaseNegociacao { get; set; }
    }
}