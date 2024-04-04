using Ineditta.Application.TemplatesEmails.Entities;

namespace Ineditta.Application.TemplatesEmails.Repositories
{
    public interface ITemplateEmailRepository
    {
        ValueTask<TemplateEmail?> ObterPorUsuarioEmailAsync(string email, TipoTemplateEmail tipoTemplate);
    }
}
