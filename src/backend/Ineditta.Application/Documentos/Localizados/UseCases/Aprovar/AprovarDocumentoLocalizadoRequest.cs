using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Documentos.Localizados.UseCases.Aprovar
{
    public sealed class AprovarDocumentoLocalizadoRequest : IRequest<Result>
    {
        public long Id { get; set; }
    }
}
