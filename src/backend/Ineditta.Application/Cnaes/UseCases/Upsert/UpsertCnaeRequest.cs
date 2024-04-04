using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Cnaes.UseCases.Upsert
{
    public class UpsertCnaeRequest : IRequest<Result>
    {
#pragma warning disable CA1805 // Do not initialize unnecessarily
        public int Id { get; set; } = 0;
#pragma warning restore CA1805 // Do not initialize unnecessarily
        public int Divisao { get; set; }
        public string DescricaoDivisao { get; set; } = null!;
        public int SubClasse { get; set; }
        public string DescricaoSubClasse { get; set; } = null!;
        public string Categoria { get; set; } = null!;
    }
}
