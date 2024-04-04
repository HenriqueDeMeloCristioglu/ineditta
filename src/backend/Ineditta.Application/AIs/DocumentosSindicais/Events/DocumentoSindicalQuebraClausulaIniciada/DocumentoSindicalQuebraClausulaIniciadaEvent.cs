using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ineditta.BuildingBlocks.Core.Bus;

namespace Ineditta.Application.AIs.DocumentosSindicais.Events.DocumentoSindicalQuebraClausulaIniciada
{
    public class DocumentoSindicalQuebraClausulaIniciadaEvent : Message
    {
        public DocumentoSindicalQuebraClausulaIniciadaEvent(long iADocumentoSindicalId)
        {
            IADocumentoSindicalId = iADocumentoSindicalId;
        }

        public long IADocumentoSindicalId { get; set; }
    }
}
