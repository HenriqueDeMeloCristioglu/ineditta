using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.CctsAssuntos.Entities;
using Ineditta.Application.Acompanhamentos.CctsEtiquetas.Entities;
using Ineditta.Application.Acompanhamentos.CctsEtiquetasOpcoes.Entities;
using Ineditta.Application.Acompanhamentos.CctsStatusOpcoes.Entities;
using Ineditta.Application.AcompanhamentosCcts.Entities;
using Ineditta.BuildingBlocks.Core.Domain.Contracts;

namespace Ineditta.Application.Acompanhamentos.Ccts.Entities
{
    public class AcompanhamentoCct : Entity, IAuditable
    {
        private List<AcompanhamentoCctAssunto> _assuntos;
        private List<AcompanhamentoCctEtiqueta> _etiquetas;

        private List<Script>? _scriptsSalvos;
        protected AcompanhamentoCct() 
        {
            _assuntos = new List<AcompanhamentoCctAssunto>();
            _etiquetas = new List<AcompanhamentoCctEtiqueta>();
        }

        private AcompanhamentoCct(DateOnly dataInicial, DateOnly? dataFinal, DateOnly? proximaLigacao, long? statusId, int usuarioResponsavelId, long? faseId, string? dataBase, IEnumerable<string>? canesIds, IEnumerable<int>? empresasIds, IEnumerable<int>? gruposEconomicosIds, int tipoDocumentoId, string? observacoesGerais, string? anotacoes, DateOnly? dataProcessamento = null, List<AcompanhamentoCctAssunto>? assuntos = null, List<AcompanhamentoCctEtiqueta>? etiquetas = null, DateOnly? validadeFinal = null)
        {
            DataInicial = dataInicial;
            DataFinal = dataFinal;
            ProximaLigacao = proximaLigacao;
            StatusId = statusId;
            UsuarioResponsavelId = usuarioResponsavelId;
            FaseId = faseId;
            DataBase = dataBase;
            CnaesIds = canesIds;
            EmpresasIds = empresasIds;
            GruposEconomicosIds = gruposEconomicosIds;
            TipoDocumentoId = tipoDocumentoId;
            ObservacoesGerais = observacoesGerais;
            Anotacoes = anotacoes;
            _scriptsSalvos = new List<Script>();
            DataProcessamento = dataProcessamento;

            _assuntos = assuntos ?? new List<AcompanhamentoCctAssunto>();
            _etiquetas = etiquetas ?? new List<AcompanhamentoCctEtiqueta>();

            ValidadeFinal = validadeFinal;
        }

        public DateOnly DataInicial { get; private set; }
        public int UsuarioResponsavelId { get; private set; }
        public int TipoDocumentoId { get; private set; }
        public DateOnly? DataFinal { get; private set; }
        public DateOnly? ProximaLigacao { get; private set; }
        public long? StatusId { get; private set; }
        public long? FaseId { get; private set; }
        public string? DataBase { get; private set; }
        public IEnumerable<string>? CnaesIds { get; private set; }
        public IEnumerable<int>? EmpresasIds { get; private set; }
        public IEnumerable<int>? GruposEconomicosIds { get; private set; }
        public string? ObservacoesGerais { get; private set; }
        public string? Anotacoes { get; private set; }
        public DateOnly? DataProcessamento { get; set; }
        public IEnumerable<Script>? ScriptsSalvos => _scriptsSalvos?.AsReadOnly();
        public IEnumerable<AcompanhamentoCctAssunto> Assuntos => _assuntos.AsReadOnly();
        public IEnumerable<AcompanhamentoCctEtiqueta> Etiquetas => _etiquetas.AsReadOnly();
        public DateOnly? ValidadeFinal { get; set; }

        public static Result<AcompanhamentoCct> Criar(DateOnly dataInicial, DateOnly? dataFinal, DateOnly? proximaLigacao, long? statusId, int usuarioResponsavelId, long? faseId, string? dataBase, IEnumerable<string>? canesIds, IEnumerable<int>? empresasIds, IEnumerable<int>? gruposEconomicosIds, int tipoDocumentoId, string? observacoesGerais, string? anotacoes, DateOnly? dataProcessamento, List<AcompanhamentoCctAssunto>? assuntos, List<AcompanhamentoCctEtiqueta>? etiquetas, DateOnly? validadeFinal)
        {
            if (dataInicial < DateOnly.MinValue)
            {
                return Result.Failure<AcompanhamentoCct>("A Data Inicial não pode ser nula");
            }

            if (usuarioResponsavelId < 0)
            {
                return Result.Failure<AcompanhamentoCct>("O Id do usuário não pode ser nulo");
            }

            if (tipoDocumentoId < 0)
            {
                return Result.Failure<AcompanhamentoCct>("O Id do tipo de documento não pode ser nulo");
            }

            var acompanhamentoCct = new AcompanhamentoCct(dataInicial, dataFinal, proximaLigacao, statusId, usuarioResponsavelId, faseId, dataBase, canesIds, empresasIds, gruposEconomicosIds, tipoDocumentoId, observacoesGerais, anotacoes, dataProcessamento, assuntos, etiquetas, validadeFinal);

            return Result.Success(acompanhamentoCct);
        }

        public Result Atualizar(DateOnly dataInicial, DateOnly? dataFinal, long? statusId, int usuarioResponsavelId, long? faseId, string? dataBase, IEnumerable<string>? canesIds, IEnumerable<int>? empresasIds, IEnumerable<int>? gruposEconomicosIds, int tipoDocumentoId, string? observacoesGerais, string? anotacoes, DateOnly? dataProcessamento, List<AcompanhamentoCctAssunto>? assuntos, List<AcompanhamentoCctEtiqueta>? etiquetas, DateOnly? validadeFinal)
        {
            if (dataInicial < DateOnly.MinValue)
            {
                return Result.Failure("A Data Inicial não pode ser nula");
            }

            if (usuarioResponsavelId < 0)
            {
                return Result.Failure("O Id do usuário não pode ser nulo");
            }

            if (tipoDocumentoId < 0)
            {
                return Result.Failure("O Id do tipo de documento não pode ser nulo");
            }

            DataInicial = dataInicial;
            DataFinal = dataFinal;
            StatusId = statusId;
            UsuarioResponsavelId = usuarioResponsavelId;
            FaseId = faseId;
            DataBase = dataBase;
            CnaesIds = canesIds;
            EmpresasIds = empresasIds;
            GruposEconomicosIds = gruposEconomicosIds;
            TipoDocumentoId = tipoDocumentoId;
            ObservacoesGerais = observacoesGerais;
            Anotacoes = anotacoes;
            DataProcessamento = dataProcessamento;
            ValidadeFinal = validadeFinal;

            if (assuntos is not null && assuntos.Any())
            {
                _assuntos.AddRange(assuntos.Where(acsl => !_assuntos.Exists(ass => acsl.EstrutucaClausulaId == ass.EstrutucaClausulaId)).ToList());

                _assuntos.RemoveAll(ast => !assuntos.Exists(ass => ast.EstrutucaClausulaId == ass.EstrutucaClausulaId));
            }
            else
            {
                _assuntos = new List<AcompanhamentoCctAssunto>();
            }

            if (etiquetas is not null && etiquetas.Any())
            {
                _etiquetas.AddRange(etiquetas.Where(acet => !_etiquetas.Exists(etq => acet.AcompanhamentoCctEtiquetaOpcaoId == etq.AcompanhamentoCctEtiquetaOpcaoId)).ToList());

                _etiquetas.RemoveAll(etq => !etiquetas.Exists(acet => etq.AcompanhamentoCctEtiquetaOpcaoId == acet.AcompanhamentoCctEtiquetaOpcaoId));
            }
            else
            {
                _etiquetas = new List<AcompanhamentoCctEtiqueta>();
            }

            return Result.Success();
        }

        public Result<Script> IncluirScript(Script script, IEnumerable<string> respostas, long status)
        {
            if (script is null)
            {
                return Result.Failure<Script>("O script não pode ser nulo");
            }

            if (respostas is null || !respostas.Any())
            {
                return Result.Failure<Script>("As Respostasa não podem ser nulas");
            }

            var isLigacao = respostas.First().Equals("Sim", StringComparison.OrdinalIgnoreCase);

            if (!isLigacao)
            {
                ProximaLigacao = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            }
            else
            {
                DateOnly interval;

                switch (status)
                {
                    case AcompanhamentoCctStatus.IndiceStatusAlta:
                    case AcompanhamentoCctStatus.IndiceStatusCliente:
                        interval = DateOnly.FromDateTime(DateTime.Now.AddDays(7));
                        break;
                    case AcompanhamentoCctStatus.IndiceStatusMedia:
                        interval = DateOnly.FromDateTime(DateTime.Now.AddDays(15));
                        break;
                    case AcompanhamentoCctStatus.IndiceStatusBaixa:
                        interval = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
                        break;
                    default:
                        interval = DateOnly.FromDateTime(DateTime.Now);
                        break;
                }

                ProximaLigacao = interval;
            }

            _scriptsSalvos ??= new List<Script>();

            _scriptsSalvos.Add(script);

            return Result.Success(script);
        }
    }
}
