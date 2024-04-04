using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NCrontab;

namespace Ineditta.BuildingBlocks.Core.HostedServices
{
    public abstract class RequestServiceBase<TRequest> : HostedServiceBase where TRequest : IRequest<Result<Unit, Error>>, new()
    {
        protected TimeSpan Interval { get; }
        protected CrontabSchedule Schedule { get; }
        protected DateTime NextExecution { get; private set; }

        protected RequestServiceBase(IServiceProvider serviceProvider, ILogger<HostedServiceBase> logger, string cronExpression) : base(serviceProvider, logger)
        {
            Schedule = CrontabSchedule.Parse(cronExpression, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
            NextExecution = Schedule.GetNextOccurrence(DateTime.Now);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                if (DateTime.Now > NextExecution)
                {
                    await ProcessAsync();

                    NextExecution = Schedule.GetNextOccurrence(DateTime.Now);
                }

                await Task.Delay(5000, stoppingToken);
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        protected virtual async ValueTask ProcessAsync() => await ProcessAsync(new TRequest());

        protected virtual async ValueTask ProcessAsync(TRequest request)
        {
            using var scope = _serviceProvider.CreateScope();

            try
            {
                var mediator = scope.ServiceProvider.GetService<IMediator>();
                var requestStateValidator = scope.ServiceProvider.GetService<RequestStateValidator>()!;

                var result = await mediator!.Send(request, CancellationToken.None);

                if (requestStateValidator.IsValid() && result.IsSuccess)
                {
                    return;
                }

                if (!requestStateValidator.IsValid())
                {
                    foreach (var error in requestStateValidator.Errors.ToList())
                    {
                        _logger.LogError("Erro ao processar requisição {Type}. Código: {Code} Mensagem {Message}", typeof(TRequest), error.Code, error.Message);
                    }

                    return;
                }

                _logger.LogError("Erro ao processar requisição {Type}. Código: {Code} Mensagem {Message}", typeof(TRequest), result.Error.Code, result.Error.Message);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Erro ao processar serviço {Name}", GetType().Name);
            }
        }
    }
}
