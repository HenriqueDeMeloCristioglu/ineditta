using CSharpFunctionalExtensions;

namespace Ineditta.Application.Documentos.Localizacoes.Entities
{
    public class DocumentoLocalizacao : Entity
    {
        private DocumentoLocalizacao(int documentoId, int localizacaoId)
        {
            DocumentoId = documentoId;
            LocalizacaoId = localizacaoId;
        }

        protected DocumentoLocalizacao() { }

        public int DocumentoId { get; private set; }
        public int LocalizacaoId { get; private set; }

        public static Result<DocumentoLocalizacao> Criar(int documentoId, int localizacaoId)
        {
            if (documentoId <= 0) return Result.Failure<DocumentoLocalizacao>("O id do documento deve ser maior que 0");
            if (localizacaoId <= 0) return Result.Failure<DocumentoLocalizacao>("O id da localizacao deve ser maior que 0");

            var documentoAbrangencia = new DocumentoLocalizacao(
                documentoId,
                localizacaoId
            );

            return Result.Success(documentoAbrangencia);
        }
    }
}
