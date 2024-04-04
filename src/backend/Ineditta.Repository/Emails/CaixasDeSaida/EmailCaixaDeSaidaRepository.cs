using CSharpFunctionalExtensions;

using Ineditta.Application.Emails.CaixasDeSaida.Entities;
using Ineditta.Application.Emails.CaixasDeSaida.Repositories;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;
using Ineditta.Repository.Contexts;
using Ineditta.Repository.Models;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.Emails.CaixasDeSaida
{
    public class EmailCaixaDeSaidaRepository : IEmailCaixaDeSaidaRepository
    {
        private readonly InedittaDbContext _context;

        public EmailCaixaDeSaidaRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask<Result> DeletarAsync(EmailCaixaDeSaida emailCaixaDeSaida)
        {
            _context.EmailCaixaDeSaida.Remove(emailCaixaDeSaida);

            await Task.CompletedTask;

            return Result.Success();
        }

        public async ValueTask<Result> IncluirAsync(EmailCaixaDeSaida emailCaixaDeSaida)
        {
            _context.EmailCaixaDeSaida.Add(emailCaixaDeSaida);

            await Task.CompletedTask;

            return Result.Success();
        }

        public async ValueTask<Result<IEnumerable<EmailCaixaDeSaida>>> ObterPorMessageId(string messageId)
        {
            var result = await _context.EmailCaixaDeSaida
                .Where(e => e.MessageId == messageId)
                .AsNoTracking()
                .ToListAsync();

            if (result is null)
            {
                return Result.Failure<IEnumerable<EmailCaixaDeSaida>>("Erro ao encontrar email na caixa de saída");
            }

            return Result.Success((IEnumerable<EmailCaixaDeSaida>)result);
        }

        public async ValueTask<Result<IEnumerable<EmailCaixaDeSaida>>> ObterTodosAsync()
        {
            var result = await _context.EmailCaixaDeSaida.ToArrayAsync();

            return Result.Success(result ?? Enumerable.Empty<EmailCaixaDeSaida>());
        }
    }
}
