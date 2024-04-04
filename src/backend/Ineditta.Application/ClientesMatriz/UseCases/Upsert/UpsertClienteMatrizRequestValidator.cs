using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;

namespace Ineditta.Application.ClientesMatriz.UseCases.Upsert
{
    public class UpsertClienteMatrizRequestValidator : AbstractValidator<UpsertClienteMatrizRequest>
    {
        public UpsertClienteMatrizRequestValidator()
        {
            RuleFor(p => p.Nome)
                .NotEmpty()
                .WithMessage("Você deve fornecer um nome");

            RuleFor(p => p.AberturaNegociacao)
                .GreaterThanOrEqualTo(0)
                .WithMessage("A 'abertura negociação' não deve ser menor que 0")
                .LessThanOrEqualTo(360)
                .WithMessage("A 'abertura negociação' não deve ser mair que 360");

            RuleFor(p => p.DataCorteForpag)
                .GreaterThanOrEqualTo(0)
                .WithMessage("A data corte fopag não deve ser menor que 0")
                .LessThanOrEqualTo(31)
                .WithMessage("A data corte fopag não deve ser maior que 31");

            RuleFor(p => p.TiposDocumentos)
                .NotNull()
                .WithMessage("Você deve fornecer uma lista de tipos de documentos")
                .Must(p => p.Any())
                .WithMessage("A lista de tipos de documentos não pode estar vazia");

            RuleFor(p => p.GrupoEconomicoId)
                .GreaterThan(0)
                .WithMessage("O id do grupo econômico deve ser maior que 0");
        }
    }
}
