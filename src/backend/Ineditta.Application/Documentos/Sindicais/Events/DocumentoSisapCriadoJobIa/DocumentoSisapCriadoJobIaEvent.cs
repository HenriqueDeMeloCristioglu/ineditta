using Ineditta.BuildingBlocks.Core.Bus;

namespace Ineditta.Application.Documentos.Sindicais.Events.DocumentoSisapCriadoJobIa
{
    public class DocumentoSisapCriadoJobIaEvent : Message
    {
        public DocumentoSisapCriadoJobIaEvent(int documentoId)
        {
            DocumentoId = documentoId;
        }

        public int DocumentoId { get; set; }
    }
}