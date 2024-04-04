using CSharpFunctionalExtensions;

namespace Ineditta.Application.Documentos.SindicatosLaborais
{
    public class DocumentoSindicatoLaboral : Entity
    {
        private DocumentoSindicatoLaboral(int documentoSindicalId, int sindicatoLaboralId)
        {
            DocumentoSindicalId = documentoSindicalId;
            SindicatoLaboralId = sindicatoLaboralId;
        }

        protected DocumentoSindicatoLaboral() { }

        public int DocumentoSindicalId { get; private set; }
        public int SindicatoLaboralId { get; private set; }

        public static Result<DocumentoSindicatoLaboral> Criar(int documentoSindicalId, int sindicatoLaboralId)
        {
            if (documentoSindicalId <= 0) return Result.Failure<DocumentoSindicatoLaboral>("O id do documento sindical deve ser maior que 0");
            if (sindicatoLaboralId <= 0) return Result.Failure<DocumentoSindicatoLaboral>("O id do sindicato laboral deve ser maior que 0");

            var documentoSindicalLaboral = new DocumentoSindicatoLaboral(documentoSindicalId, sindicatoLaboralId);

            return Result.Success(documentoSindicalLaboral);
        }
    }
}
