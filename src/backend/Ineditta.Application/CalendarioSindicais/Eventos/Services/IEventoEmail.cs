using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Ineditta.Application.CalendarioSindicais.Eventos.Dtos;
using Ineditta.BuildingBlocks.Core.Domain.Models;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Services
{
    public interface IEventoEmail
    {
        ValueTask<Result<bool, Error>> EnviarNotificacaoVencimentoDocumentoAsync(List<string> emails, VencimentoDocumentoViewModel evento, CancellationToken cancellationToken = default);
        ValueTask<Result<bool, Error>> EnviarNotificacaoVencimentoMandatoPatronalAsync(List<string> emails, VencimentoMandatoPatronalViewModel vencimentoMandatoPatronal, CancellationToken cancellationToken = default);
        ValueTask<Result<bool, Error>> EnviarNotificacaoVencimentoMandatoLaboralAsync(List<string> emails, VencimentoMandatoLaboralViewModel vencimentoMandatoLaboral, CancellationToken cancellationToken = default);
        ValueTask<Result<bool, Error>> EnviarNotificacaoEventoClausulaAsync(List<string> emails, EventoClausulaViewModel eventoClausula, CancellationToken cancellationToken = default);
        ValueTask<Result<bool, Error>> EnviarNotificacaoAgendaEventosAsync(List<string> emails, AgendaEventosViewModel eventoAgenda, CancellationToken cancellationToken = default);
        ValueTask<Result<bool, Error>> EnviarNotificacaoEventoTrintidioAsync(List<string> emails, EventoTrintidioViewModel evento, CancellationToken cancellationToken = default);
        ValueTask<Result<bool, Error>> EnviarNotificacaoAssembleiaPatronalAsync(List<string> emails, AssembleiaReuniaoViewModelBase evento, CancellationToken cancellationToken = default);
        ValueTask<Result<bool, Error>> EnviarNotificacaoReuniaoSindicalAsync(List<string> emails, AssembleiaReuniaoViewModelBase evento, CancellationToken cancellationToken = default);
    }
}
