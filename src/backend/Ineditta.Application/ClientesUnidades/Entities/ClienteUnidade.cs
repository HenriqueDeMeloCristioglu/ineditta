using CSharpFunctionalExtensions;

using Ineditta.Application.SharedKernel.Logradouros.ValueObjects;
using Ineditta.BuildingBlocks.Core.Domain.Contracts;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

namespace Ineditta.Application.ClientesUnidades.Entities
{
    public class ClienteUnidade : Entity<int>, IAuditable
    {
        private List<CnaeUnidade>? _cnaesUnidades;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected ClienteUnidade()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            
        }

        private ClienteUnidade(string codigo, string nome, CNPJ cnpj, Logradouro logradouro, DateOnly? dataAusencia, string? codigoSindicatoCliente, string? codigoSindicatoPatronal, int empresaId, int tipoNegocioId, int localizacaoId, int? cnaeFilial, IEnumerable<CnaeUnidade>? cnaesUnidade)
        {
            Codigo = codigo;
            Nome = nome;
            Cnpj = cnpj;
            Logradouro = logradouro;
            DataAusencia = dataAusencia;
            CodigoSindicatoCliente = codigoSindicatoCliente;
            CodigoSindicatoPatronal = codigoSindicatoPatronal;
            EmpresaId = empresaId;
            TipoNegocioId = tipoNegocioId;
            LocalizacaoId = localizacaoId;
            CnaeFilial = cnaeFilial;
            _cnaesUnidades = cnaesUnidade?.ToList();
        }

        public string Codigo { get; private set; } = null!;
        public string Nome { get; private set; } = null!;
        public CNPJ Cnpj { get; private set; } = null!;
        public Logradouro Logradouro { get; private set; }
        public DateOnly? DataAusencia { get; private set; }
        public string? CodigoSindicatoCliente { get; private set; }
        public string? CodigoSindicatoPatronal { get; private set; }
        public int EmpresaId { get; private set; }
        public int TipoNegocioId { get; private set; }
        public int LocalizacaoId { get; private set; }
        public int? CnaeFilial { get; private set; }

        public IEnumerable<CnaeUnidade>? CnaesUnidades => _cnaesUnidades?.AsReadOnly();

        internal static Result<ClienteUnidade> Criar(string codigo, string nome, CNPJ cnpj, Logradouro logradouro, DateOnly? dataAusencia, string? codigoSindicatoCliente, string? codigoSindicatoPatronal, int empresaId, int tipoNegocioId, int localizacaoId, int? cnaeFilial, IEnumerable<CnaeUnidade> cnaesUnidade)
        {
            if (codigo is null) return Result.Failure<ClienteUnidade>("Informe o Código");
            if (nome is null) return Result.Failure<ClienteUnidade>("Informe o Nome");
            if (cnpj is null) return Result.Failure<ClienteUnidade>("Informe o Cnpj");
            if (empresaId == 0) return Result.Failure<ClienteUnidade>("Informe o Id da empresa");
            if (tipoNegocioId == 0) return Result.Failure<ClienteUnidade>("Informe o Id do tipo de negocio");
            if (localizacaoId == 0) return Result.Failure<ClienteUnidade>("Informe o Id da localização");

            var clienteUnidade = new ClienteUnidade(
                codigo,
                nome,
                cnpj,
                logradouro,
                dataAusencia,
                codigoSindicatoCliente,
                codigoSindicatoPatronal,
                empresaId,
                tipoNegocioId,
                localizacaoId,
                cnaeFilial,
                cnaesUnidade);
            
            return Result.Success(clienteUnidade);
        }

        internal Result Atualizar(string codigo, string nome, CNPJ cnpj, Logradouro logradouro, DateOnly? dataAusencia, string? codigoSindicatoCliente, string? codigoSindicatoPatronal, int empresaId, int tipoNegocioId, int localizacaoId, int? cnaeFilial, IEnumerable<CnaeUnidade> cnaesUnidade)
        {
            if (codigo is null) return Result.Failure<ClienteUnidade>("Informe o Código");
            if (nome is null) return Result.Failure<ClienteUnidade>("Informe o Nome");
            if (cnpj is null) return Result.Failure<ClienteUnidade>("Informe o Cnpj");
            if (empresaId == 0) return Result.Failure<ClienteUnidade>("Informe o Id da empresa");
            if (tipoNegocioId == 0) return Result.Failure<ClienteUnidade>("Informe o Id do tipo de negocio");
            if (localizacaoId == 0) return Result.Failure<ClienteUnidade>("Informe o Id da localização");

            Codigo = codigo;
            Nome = nome;
            Cnpj = cnpj;
            Logradouro = logradouro;
            DataAusencia = dataAusencia;
            CodigoSindicatoCliente = codigoSindicatoCliente;            
            CodigoSindicatoPatronal = codigoSindicatoPatronal;
            EmpresaId = empresaId;
            TipoNegocioId= tipoNegocioId;
            LocalizacaoId = localizacaoId;
            CnaeFilial = cnaeFilial;
            _cnaesUnidades = cnaesUnidade?.ToList() ?? new List<CnaeUnidade>();

            return Result.Success();
        }
    }
}
