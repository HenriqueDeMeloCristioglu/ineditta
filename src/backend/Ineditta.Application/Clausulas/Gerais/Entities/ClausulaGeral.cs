using CSharpFunctionalExtensions;

namespace Ineditta.Application.Clausulas.Gerais.Entities
{
    public class ClausulaGeral : Entity<int>
    {
        protected ClausulaGeral()
        {
        }

        private ClausulaGeral(string? texto, int documentoSindicalId, int estruturaClausulaId, int? numero, int? assuntoId, int? sinonimoId, DateOnly? dataProcessamento, int? responsavelProcessamento, string? textoResumido, bool constaNoDocumento, ResumoStatus? resumoStatus, bool aprovado)
        {
            Texto = texto;
            DocumentoSindicalId = documentoSindicalId;
            EstruturaClausulaId = estruturaClausulaId;
            Numero = numero;
            AssuntoId = assuntoId;
            SinonimoId = sinonimoId;
            DataProcessamento = dataProcessamento;
            ResponsavelProcessamento = responsavelProcessamento;
            TextoResumido = textoResumido;
            ConstaNoDocumento = constaNoDocumento;
            ResumoStatus = resumoStatus;
        }

        public string? Texto { get; private set; }
        public int DocumentoSindicalId { get; private set; }
        public int EstruturaClausulaId { get; private set; }
        public int? UsuarioAprovadorId { get; private set; }
        public DateOnly? DataProcessamento { get; private set; }
        public int? ResponsavelProcessamento { get; private set; }
        public int? Numero { get; private set; }
        public DateOnly? DataAprovacao { get; private set; }
        public bool? Aprovado { get; private set; }
        public bool? Liberado { get; private set; }
        public int? AssuntoId { get; private set; }
        public int? SinonimoId { get; private set; }
        public string? TextoResumido { get; private set; }
        public bool ConstaNoDocumento { get; private set; }
        public DateOnly? DataProcessamentoDocumento { get; private set; }
        public ResumoStatus? ResumoStatus { get; private set; }

        public static Result<ClausulaGeral> Criar(string? texto, int documentoSindicalId, int estruturaClausulaId, int? numero, int? assuntoId, int? sinonimoId, int? responsavelProcessamento, string? textoResumido, bool constaNoDocumento, bool aprovado)
        {
            if (documentoSindicalId <= 0)
            {
                return Result.Failure<ClausulaGeral>("O Id do Documento não pode ser nulo");
            }

            if (estruturaClausulaId <= 0)
            {
                return Result.Failure<ClausulaGeral>("O Id da Estrutura Clausulas não pode ser nulo");
            }

            var clausulasGeral = new ClausulaGeral(texto, documentoSindicalId, estruturaClausulaId, numero, assuntoId, sinonimoId, DateOnly.FromDateTime(DateTime.Now), responsavelProcessamento, textoResumido, constaNoDocumento, Entities.ResumoStatus.NaoResumida, aprovado);

            return Result.Success(clausulasGeral);
        }

        public Result Atualizar(string? texto, int documentoSindicalId, int estruturaClausulaId, int? numero, int? assuntoId, int? sinonimoId, bool constaNoDocumento)
        {
            if (documentoSindicalId <= 0)
            {
                return Result.Failure("O Id do Documento não pode ser nulo");
            }

            if (estruturaClausulaId <= 0)
            {
                return Result.Failure("O Id da Estrutura Clausulas não pode ser nulo");
            }

            Texto = texto;
            DocumentoSindicalId = documentoSindicalId;
            EstruturaClausulaId = estruturaClausulaId;
            Numero = numero;
            AssuntoId = assuntoId;
            SinonimoId = sinonimoId;
            ConstaNoDocumento = constaNoDocumento;

            return Result.Success();
        }

        public Result Aprovar(int usuarioAprovadorId)
        {
            if (usuarioAprovadorId <= 0)
            {
                return Result.Failure("O Id do Usuário Aprovador não pode ser nulo");
            }

            DataAprovacao = DateOnly.FromDateTime(DateTime.Now);
            Aprovado = true;
            UsuarioAprovadorId = usuarioAprovadorId;

            return Result.Success();
        }

        public Result Liberar()
        {
            Liberado = true;
            DataProcessamentoDocumento = DateOnly.FromDateTime(DateTime.Now);

            return Result.Success();
        }
    
        public Result AtualizarResumo(string textoResumido)
        {
            TextoResumido = textoResumido;
            ResumoStatus = Entities.ResumoStatus.Resumido;

            return Result.Success();
        }

        public Result Resumir()
        {
            ResumoStatus = Entities.ResumoStatus.Resumindo;

            return Result.Success();
        }
    }
}
