using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

namespace Ineditta.Application.TiposDocumentosClientesMatriz.Entities
{
    public class TipoDocumentoClienteMatriz : Entity
    {
        private TipoDocumentoClienteMatriz(int tipoDocumentoId, int clienteMatrizId)
        {
            TipoDocumentoId = tipoDocumentoId;
            ClienteMatrizId = clienteMatrizId;
        }

        protected TipoDocumentoClienteMatriz()
        {}
        
        public int TipoDocumentoId { get; set; }
        public int ClienteMatrizId { get; set; }

        public static Result<TipoDocumentoClienteMatriz> Criar(int tipoDocumentoId, int clienteMatrizId)
        {
            if (tipoDocumentoId <= 0) return Result.Failure<TipoDocumentoClienteMatriz>("O id do tipo de documento deve ser maior que 0");
            if (clienteMatrizId <= 0) return Result.Failure<TipoDocumentoClienteMatriz>("O id do cliente matriz deve ser maio que 0");

            var tipoDocumentoClienteMatriz = new TipoDocumentoClienteMatriz(tipoDocumentoId, clienteMatrizId);

            return Result.Success(tipoDocumentoClienteMatriz);
        }
    }
}
