using CSharpFunctionalExtensions;

using Ineditta.Application.Usuarios.Entities;

namespace Ineditta.Application.TemplatesEmails.Entities
{
    public class TemplateEmail : Entity<int>
    {
        private TemplateEmail(TipoTemplateEmail tipoTemplate, Nivel nivel, int referenciaId, string html)
        {
            TipoTemplate = tipoTemplate;
            Nivel = nivel;
            ReferenciaId = referenciaId;
            Html = html;
        }

        public TemplateEmail() { }

        public TipoTemplateEmail TipoTemplate { get; private set; }
        public Nivel Nivel { get; private set; }
        public int ReferenciaId { get; private set; }
        public string Html { get; private set; } = null!;

        public static Result<TemplateEmail> Criar(TipoTemplateEmail tipoTemplate, Nivel nivel, int referenciaId, string html)
        {
            if (referenciaId <= 0) return Result.Failure<TemplateEmail>("O id do grupo econômico/matriz/estabelecimento deve ser maior que 0");
            if (string.IsNullOrEmpty(html)) return Result.Failure<TemplateEmail>("O template do email não pode ser vazio");

            var template = new TemplateEmail(tipoTemplate, nivel, referenciaId, html);

            return Result.Success(template);
        }

        public Result Atualizar(TipoTemplateEmail tipoTemplate, Nivel nivel, int referenciaId, string html)
        {
            if (referenciaId <= 0) return Result.Failure<TemplateEmail>("O id do grupo econômico/matriz/estabelecimento deve ser maior que 0");
            if (string.IsNullOrEmpty(html)) return Result.Failure<TemplateEmail>("O template do email não pode ser vazio");

            TipoTemplate = tipoTemplate;
            Nivel = nivel;
            ReferenciaId = referenciaId;
            Html = html;

            return Result.Success();
        }
    }
}
