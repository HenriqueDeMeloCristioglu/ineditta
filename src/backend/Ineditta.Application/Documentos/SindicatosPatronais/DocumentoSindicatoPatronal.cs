using CSharpFunctionalExtensions;

namespace Ineditta.Application.Documentos.SindicatosPatronais
{
    public class DocumentoSindicatoPatronal : Entity
    {
        private DocumentoSindicatoPatronal(int documentoSindicalId, int sindicatoPatronalId)
        {
            DocumentoSindicalId = documentoSindicalId;
            SindicatoPatronalId = sindicatoPatronalId;
        }

        protected DocumentoSindicatoPatronal() { }

        public int DocumentoSindicalId { get; private set; }
        public int SindicatoPatronalId { get; private set; }

        public static Result<DocumentoSindicatoPatronal> Criar(int documentoSindicalId, int sindicatoPatronalId)
        {
            if (documentoSindicalId <= 0) return Result.Failure<DocumentoSindicatoPatronal>("O id do documento sindical deve ser maior que 0");
            if (sindicatoPatronalId <= 0) return Result.Failure<DocumentoSindicatoPatronal>("O id do sindicato laboral deve ser maior que 0");

            var documentoSindicalLaboral = new DocumentoSindicatoPatronal(documentoSindicalId, sindicatoPatronalId);

            return Result.Success(documentoSindicalLaboral);
        }
    }
}
