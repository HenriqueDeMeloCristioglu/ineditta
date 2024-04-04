using Ineditta.Application.Emails.StoragesManagers.Entities;
using Ineditta.Application.Emails.StoragesManagers.Repositories;
using Ineditta.Repository.Contexts;

namespace Ineditta.Repository.Emails.StoragesManagers
{
    public class EmailStorageManagerRepository : IEmailStorageManagerRepository
    {
        private readonly InedittaDbContext _context;

        public EmailStorageManagerRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask IncluirAsync(EmailStorageManager emailStorageManager)
        {
            _context.Add(emailStorageManager);

            await Task.CompletedTask;
        }
    }
}
