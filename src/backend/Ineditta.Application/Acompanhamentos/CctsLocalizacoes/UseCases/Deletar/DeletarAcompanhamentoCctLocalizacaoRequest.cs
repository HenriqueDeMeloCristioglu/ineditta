using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.CctsLocalizacoes.Entities;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.CctsLocalizacoes.UseCases.Deletar
{
    public class DeletarAcompanhamentoCctLocalizacaoRequest : IRequest<Result>
    {
        public AcompanhamentoCctLocalizacao AcompanhamentoCctLocalizacao { get; set; } = null!;
    }
}
