using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.InformacoesAdicionais.Cliente.Aprovar
{
    public class AprovarInformacaoAdicionalClienteRequest : IRequest<Result>
    {
        public int DocumentoSindicalId { get; set; }
    }
}
