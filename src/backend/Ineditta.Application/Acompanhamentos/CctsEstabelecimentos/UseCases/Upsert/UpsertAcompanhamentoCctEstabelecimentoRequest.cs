using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.CctsEstabelecimentos.UseCases.Upsert
{
    public class UpsertAcompanhamentoCctEstabelecimentoRequest : IRequest<Result>
    {
        public int AcompanhamentoCctId { get; set; }
        public int GrupoEconomicoId { get; set; }
        public int EmpresaId { get; set; }
        public int EstabelecimentoId { get; set; }
    }
}
