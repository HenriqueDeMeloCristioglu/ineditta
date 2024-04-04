using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.Contracts;

namespace Ineditta.Application.AIs.DocumentosSindicais.Entities
{
    public class IADocumentoSindical : Entity, IAuditable
    {
        private IADocumentoSindical(int documentoReferenciaId, IADocumentoStatus status, IADocumentoStatus ultimoStatusProcessado, DateOnly? dataAprovacao = null, int? usuarioAprovaorId = null)
        {
            DocumentoReferenciaId = documentoReferenciaId;
            Status = status;
            UltimoStatusProcessado = ultimoStatusProcessado;
            DataAprovacao = dataAprovacao;
            UsuarioAprovaorId = usuarioAprovaorId;
        }

        public int DocumentoReferenciaId { get; private set; }
        public IADocumentoStatus Status { get; private set; }
        public IADocumentoStatus UltimoStatusProcessado { get; private set; }
        public string? MotivoErro { get; private set; }
        public DateOnly? DataAprovacao { get; private set; }
        public int? UsuarioAprovaorId { get; private set; }

        public static Result<IADocumentoSindical> Criar(int documentoReferenciaId, IADocumentoStatus status)
        {
            if (documentoReferenciaId <= 0)
            {
                return Result.Failure<IADocumentoSindical>("Id inválido");
            }

            if (status <= 0)
            {
                return Result.Failure<IADocumentoSindical>("Status inválido");
            }

            var iaDocumentoSindical = new IADocumentoSindical(documentoReferenciaId, status, status);

            return Result.Success(iaDocumentoSindical);
        }

        public Result Atualizar(int documentoReferenciaId, IADocumentoStatus status, string? motivoErro)
        {
            if (documentoReferenciaId <= 0)
            {
                return Result.Failure("Id inválido");
            }

            if (status <= 0)
            {
                return Result.Failure("Status inválido");
            }

            if (status != IADocumentoStatus.Erro)
            {
                UltimoStatusProcessado = status;
            }

            DocumentoReferenciaId = documentoReferenciaId;
            Status = status;
            MotivoErro = motivoErro;

            return Result.Success();
        }

        public void QuebrarClausulas()
        {
            Status = IADocumentoStatus.QuebrandoClausulas;
            UltimoStatusProcessado = IADocumentoStatus.QuebrandoClausulas;
        }

        public void ConfirmarQuebraClausula(bool necessitaAprovacao = true)
        {
            Status = necessitaAprovacao ? IADocumentoStatus.AguardandoAprovacaoQuebraClausula : IADocumentoStatus.ClassificandoClausulas;
            UltimoStatusProcessado = necessitaAprovacao ? IADocumentoStatus.AguardandoAprovacaoQuebraClausula : IADocumentoStatus.ClassificandoClausulas;
        }

        public void ClassificarClausula()
        {
            Status = IADocumentoStatus.ClassificandoClausulas;
            UltimoStatusProcessado = IADocumentoStatus.ClassificandoClausulas;
        }

        public void ConfirmarClassificacaoClausula()
        {
            Status = IADocumentoStatus.AguardandoAprovacaoClassificacao;
            UltimoStatusProcessado = IADocumentoStatus.AguardandoAprovacaoClassificacao;
        }

        public void Aprovar(int? usuarioAprovaorId)
        {
            Status = IADocumentoStatus.Aprovado;
            UltimoStatusProcessado = IADocumentoStatus.Aprovado;
            DataAprovacao = DateOnly.FromDateTime(DateTime.Now);
            UsuarioAprovaorId = usuarioAprovaorId;
        }

        public void RegistrarErro(string motivoErro)
        {
            if (Status != IADocumentoStatus.Erro)
            {
                UltimoStatusProcessado = Status;
            }
            
            Status = IADocumentoStatus.Erro;
            MotivoErro = string.IsNullOrEmpty(motivoErro) ? "Ocorreu um erro inesperado durante o processamento." : motivoErro;
        }

        public void Reprocessar()
        {
            Status = UltimoStatusProcessado;
            MotivoErro = null;
        }

        public bool Erro()
        {
            return Status == IADocumentoStatus.Erro;
        }
    }
}
