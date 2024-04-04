using CSharpFunctionalExtensions;

namespace Ineditta.Application.Documentos.AtividadesEconomicas.Entities
{
    public class DocumentoAtividadeEconomica: Entity
    {
        protected DocumentoAtividadeEconomica() { }
        private DocumentoAtividadeEconomica(int documentoId, int atividadeEconomicaId)
        {
            DocumentoId = documentoId;
            AtividadeEconomicaId = atividadeEconomicaId;
        }

        public int DocumentoId { get; private set; }
        public int AtividadeEconomicaId { get; private set; }

        internal static Result<DocumentoAtividadeEconomica> Criar(int documentoId, int atividadeEconomicaId)
        {
            if (documentoId <= 0)
            {
                return Result.Failure<DocumentoAtividadeEconomica>("Id do Documento inválido");
            }

            if (atividadeEconomicaId <= 0)
            {
                return Result.Failure<DocumentoAtividadeEconomica>("Id da Atividade Econômica inválido");
            }

            var documentoAtividadeEconomica = new DocumentoAtividadeEconomica(documentoId, atividadeEconomicaId);

            return documentoAtividadeEconomica;
        }
    }
}
