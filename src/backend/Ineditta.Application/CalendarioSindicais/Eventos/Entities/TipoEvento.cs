using System.ComponentModel;

using Ineditta.BuildingBlocks.Core.Web.API.Binders.EnumAsString;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Entities
{
    [BindAsString]
    public enum TipoEvento
    {
        [Description("Vencimento de Documento")]
        VencimentoDeDocumento = 1,

        [Description("Vencimento Mandato Sindical Laboral")]
        VencimentoMandatoSindicalLaboral,

        [Description("Vencimento Mandato Sindical Patronal")]
        VencimentoMandatoSindicalPatronal,

        [Description("Trintidio")]
        Trintidio,

        [Description("Eventos de Cláusulas")]
        EventoClausula,

        [Description("Agenda de Eventos")]
        AgendaEventos,

        [Description("Assembléia Patronal")]
        AssembleiaPatronal = 7,

        [Description("Reunião entre as partes")]
        ReuniaoEntrePartes = 8
    }
}
