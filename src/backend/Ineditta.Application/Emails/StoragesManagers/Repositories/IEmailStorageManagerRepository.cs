using Ineditta.Application.Emails.StoragesManagers.Entities;

namespace Ineditta.Application.Emails.StoragesManagers.Repositories
{
    public interface IEmailStorageManagerRepository
    {
        ValueTask IncluirAsync(EmailStorageManager emailStorageManager);
    }
}
