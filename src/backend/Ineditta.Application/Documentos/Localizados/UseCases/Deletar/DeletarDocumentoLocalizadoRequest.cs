using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Documentos.Localizados.UseCases.Deletar
{
    public sealed class DeletarDocumentoLocalizadoRequest : IRequest<Result>
    {
        public long Id { get; set; }
    }
}
