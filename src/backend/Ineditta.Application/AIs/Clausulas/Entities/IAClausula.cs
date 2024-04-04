using CSharpFunctionalExtensions;

using Ineditta.Application.EstruturasClausulas.Gerais.Entities;
using Ineditta.Application.Sinonimos.Entities;

namespace Ineditta.Application.AIs.Clausulas.Entities
{
    public class IAClausula : Entity<int>
    {
        private IAClausula(string? nome, string texto, string? grupo, string? subGrupo, long iADocumentoSindicalId, int? estruturaClausulaId, DateOnly dataProcessamento, int numero, int? sinonimoId, IAClausulaStatus status)
        {
            Nome = nome;
            Texto = texto;
            Grupo = grupo;
            SubGrupo = subGrupo;
            IADocumentoSindicalId = iADocumentoSindicalId;
            EstruturaClausulaId = estruturaClausulaId;
            DataProcessamento = dataProcessamento;
            Numero = numero;
            SinonimoId = sinonimoId;
            Status = status;
        }

        private IAClausula() { }

        public string? Nome { get; private set; }
        public string Texto { get; private set; } = null!;
        public string? Grupo { get; private set; }
        public string? SubGrupo { get; private set; }
        public long IADocumentoSindicalId { get; private set; }
        public int? EstruturaClausulaId { get; private set; }
        public DateOnly DataProcessamento { get; private set; }
        public int Numero { get; private set; }
        public int? SinonimoId { get; private set; }
        public IAClausulaStatus Status { get; private set; }

        public static Result<IAClausula> Criar(string? nome, string texto, string? grupo, string? subGrupo, long iADocumentoSindicalId, int? estruturaClausulaId, int numero, int? sinonimoId, IAClausulaStatus? status)
        {
            if (string.IsNullOrEmpty(texto)) return Result.Failure<IAClausula>("O texto da ia_clausula não pode ser nulo.");
            if (iADocumentoSindicalId <= 0) return Result.Failure<IAClausula>("O id do documento sindical tem que ser maior que 0");
            if (estruturaClausulaId <= 0) return Result.Failure<IAClausula>("O id da estrutura de clausula tem que ser maior que 0");
            if (numero < 0) return Result.Failure<IAClausula>("O numero da calusula não pode ser negativo");
            if (sinonimoId <= 0) return Result.Failure<IAClausula>("O sinonimo id da ia_clausula deve ser maior que 0");

            var iaClausula = new IAClausula(
                nome,
                texto,
                grupo,
                subGrupo,
                iADocumentoSindicalId,
                estruturaClausulaId,
                DateOnly.FromDateTime(DateTime.Now),
                numero,
                sinonimoId,
                status ?? IAClausulaStatus.Consistente
            );

            return Result.Success(iaClausula);
        }

        public Result Atualizar(string texto, long iADocumentoSindicalId, int estruturaClausulaId, int numero, int sinonimoId, IAClausulaStatus? status)
        {
            if (string.IsNullOrEmpty(texto))
            {
                return Result.Failure<IAClausula>("O texto da ia_clausula não pode ser nulo.");
            }

            if (iADocumentoSindicalId <= 0)
            {
                return Result.Failure<IAClausula>("O id do documento sindical tem que ser maior que 0");
            }

            if (estruturaClausulaId <= 0)
            {
                return Result.Failure<IAClausula>("O id da estrutura de clausula tem que ser maior que 0");
            }

            if (numero < 0)
            {
                return Result.Failure<IAClausula>("O numero da calusula não pode ser negativo");
            }

            if (sinonimoId <= 0)
            {
                return Result.Failure<IAClausula>("O sinonimo id da ia_clausula deve ser maior que 0");
            }

            Texto = texto;
            IADocumentoSindicalId = iADocumentoSindicalId;
            EstruturaClausulaId = estruturaClausulaId;
            Numero = numero;
            SinonimoId = sinonimoId;

            if (status != null)
            {
                if (!Enum.IsDefined(typeof(IAClausulaStatus), status))
                {
                    return Result.Failure<IAClausula>("O status informado é inválido");
                }

                Status = (IAClausulaStatus)status;
            }

            return Result.Success();
        }

        public Result Classificar(int? estruturaClausulaId, int? sinonimoId, IAClausulaStatus status = IAClausulaStatus.Consistente)
        {
            if (!Enum.IsDefined(typeof(IAClausulaStatus), status))
            {
                return Result.Failure<IAClausula>("O status informado é inválido");
            }

            if ((estruturaClausulaId ?? 0) > 0)
            {
                EstruturaClausulaId = estruturaClausulaId;
            }

            if ((sinonimoId ?? 0) > 0)
            {
                SinonimoId = sinonimoId;
            }

            Status = status;

            return Result.Success();
        }

        public Result LimparClassificacao()
        {
            EstruturaClausulaId = null;
            SinonimoId = null;
            Status = IAClausulaStatus.Inconsistente;

            return Result.Success();
        }

        public Result AtualizarStatus(IAClausulaStatus status)
        {
            if (!Enum.IsDefined(typeof(IAClausulaStatus), status))
            {
                return Result.Failure<IAClausula>("O status informado é inválido");
            }

            Status = status;

            return Result.Success();
        }
    }
}
