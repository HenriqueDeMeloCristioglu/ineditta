using FluentValidation;

namespace Ineditta.Application.Acompanhamentos.Ccts.UseCases.Upsert
{
    public class UpsertAcompanhamentoCctRequestValidator : AbstractValidator<UpsertAcompanhamentoCctRequest>
    {
        public UpsertAcompanhamentoCctRequestValidator()
        {
            RuleFor(a => a.DataInicial)
                .GreaterThan(DateOnly.MinValue)
                .WithMessage("Data Inicial inválida");

            RuleFor(a => a.UsuarioResponsavelId)
                .NotNull()
                .WithMessage("Id do Usuário Responsável não pode ser nulo")
                .GreaterThan(0)
                .WithMessage("Id do Usuário Responsável deve ser maior que 0");

            RuleFor(a => a.SindicatosLaboraisIds)
                .NotNull()
                .WithMessage("Id do Sindicato Laboral não pode ser nulo");

            RuleFor(a => a.TipoDocumentoId)
                .NotNull()
                .WithMessage("Id do Tipo de Documento não pode ser nulo")
                .GreaterThan(0)
                .WithMessage("Id do Tipo de Documento deve ser maior que 0");
        }
    }
}
