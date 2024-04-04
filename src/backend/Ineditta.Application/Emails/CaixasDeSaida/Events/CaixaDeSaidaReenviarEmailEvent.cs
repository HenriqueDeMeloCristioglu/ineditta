using Ineditta.Application.Emails.CaixasDeSaida.Entities;
using Ineditta.BuildingBlocks.Core.Bus;

namespace Ineditta.Application.Emails.CaixasDeSaida.Events
{
    public class CaixaDeSaidaReenviarEmailEvent : Message
    {
        public CaixaDeSaidaReenviarEmailEvent(IEnumerable<EmailCaixaDeSaida> emails)
        {
            Emails = emails;
        }

        public IEnumerable<EmailCaixaDeSaida> Emails { get; set; }
    }
}
