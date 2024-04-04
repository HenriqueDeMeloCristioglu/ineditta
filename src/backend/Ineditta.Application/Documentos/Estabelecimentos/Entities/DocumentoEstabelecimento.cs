using CSharpFunctionalExtensions;

namespace Ineditta.Application.Documentos.Estabelecimentos.Entities
{
    public class DocumentoEstabelecimento : Entity
    {
        private DocumentoEstabelecimento(int documentoId, int estabelecimentoId)
        {
            DocumentoId = documentoId;
            EstabelecimentoId = estabelecimentoId;
        }

        protected DocumentoEstabelecimento() { }

        public int DocumentoId { get; private set; }
        public int EstabelecimentoId { get; private set; }

        public static Result<DocumentoEstabelecimento> Criar(int documentoId, int estabelecimentoId)
        {
            if (documentoId <= 0) return Result.Failure<DocumentoEstabelecimento>("O id do documento deve ser maior que 0");
            if (estabelecimentoId <= 0) return Result.Failure<DocumentoEstabelecimento>("O id do estabelecimento deve ser maior que 0");

            var documentoEstabelecimento = new DocumentoEstabelecimento(documentoId, estabelecimentoId);

            return Result.Success(documentoEstabelecimento);
        }
    }
}
