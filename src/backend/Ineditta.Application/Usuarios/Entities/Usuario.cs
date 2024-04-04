using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.Contracts;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

namespace Ineditta.Application.Usuarios.Entities
{
    public class Usuario : Entity<int>, IAuditable
    {
        private List<UsuarioModulo>? _modulosSISAP;
        private List<UsuarioModulo>? _modulosComerciais;
        
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Usuario()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        private Usuario(
        string nome,
        Email email,
        string cargo,
        string? celular,
        string? ramal,
        int? idSuperior,
        string? departamento,
        bool bloqueado,
        bool documentoRestrito,
        Ausencia? ausencia,
        string? tipo,
        Nivel nivel,
        bool notificarWhatsapp,
        bool notificarEmail,
        int? idGrupoEconomico,
        int? idJornada,
        int[]? localidadesIds,
        int[]? cnaesIds,
        int[]? grupoClausulasIds,
        int[]? estabelecimentosIds,
        IEnumerable<UsuarioModulo>? modulosSISAP,
        IEnumerable<UsuarioModulo>? modulosComerciais)
        {
            Nome = nome;
            Email = email;
            Cargo = cargo;
            Celular = celular;
            Ramal = ramal;
            IdSuperior = idSuperior;
            Departamento = departamento;
            Bloqueado = bloqueado;
            DocumentoRestrito = documentoRestrito;
            Ausencia = ausencia;
            Tipo = tipo;
            Nivel = nivel;
            NotificarWhatsapp = notificarWhatsapp;
            NotificarEmail = notificarEmail;
            GrupoEconomicoId = idGrupoEconomico;
            JornadaId = idJornada;
            LocalidadesIds = localidadesIds;
            CnaesIds = cnaesIds;
            GruposClausulasIds = grupoClausulasIds;
            EstabelecimentosIds = estabelecimentosIds;
            _modulosSISAP = modulosSISAP?.ToList() ?? new List<UsuarioModulo>();
            _modulosComerciais = modulosComerciais?.ToList() ?? new List<UsuarioModulo>();
        }

        public string Nome { get; private set; }
        public Email Email { get; private set; }
        public string Cargo { get; private set; }
        public string? Foto { get; private set; }
        public string? Celular { get; private set; }
        public string? Ramal { get; private set; }
        public int? IdSuperior { get; private set; }
        public string? Departamento { get; private set; }
        public bool Bloqueado { get; private set; }
        public bool DocumentoRestrito { get; private set; }
        public Ausencia? Ausencia { get; private set; }
        public string? Tipo { get; private set; }
        public Nivel Nivel { get; private set; }
        public bool NotificarWhatsapp { get; private set; }
        public bool NotificarEmail { get; private set; }
        public int? GrupoEconomicoId { get; private set; }
        public int? JornadaId { get; private set; }
        public int[]? LocalidadesIds { get; private set; }
        public int[]? CnaesIds { get; private set; }
        public int[]? GruposClausulasIds { get; private set; }
        public int[]? EstabelecimentosIds { get; private set; }
        public bool TrocaSenhaLogin { get; private set; }

        public IEnumerable<UsuarioModulo>? ModulosSISAP => _modulosSISAP?.AsReadOnly();
        public IEnumerable<UsuarioModulo>? ModulosComerciais => _modulosComerciais?.AsReadOnly();

        public static Result<Usuario> Criar(
            string nome,
            Email email,
            string cargo,
            string? celular,
            string? ramal,
            int? idSuperior,
            string? departamento,
            bool bloqueado,
            bool documentoRestrito,
            Ausencia? ausencia,
            string? tipo,
            Nivel nivel,
            bool notificarWhatsapp,
            bool notificarEmail,
            int? grupoEconomicoId,
            int? idJornada,
            int[]? localidadesIds,
            int[]? cnaesIds,
            int[]? gruposClausulasIds,
            int[]? estabelecimentosIds,
            IEnumerable<UsuarioModulo>? modulosSISAP,
            IEnumerable<UsuarioModulo>? modulosComerciais
            )
        {
            if (nome is null) return Result.Failure<Usuario>("Informe o Nome");
            if (email is null) return Result.Failure<Usuario>("Informe o Email");
            if (cargo is null) return Result.Failure<Usuario>("Informe o Cargo");

            var usuario = new Usuario(
                nome,
                email,
                cargo,
                celular,
                ramal,
                idSuperior,
                departamento,
                bloqueado,
                documentoRestrito,
                ausencia ?? Ausencia.SemAusencia(),
                tipo,
                nivel,
                notificarWhatsapp,
                notificarEmail,
                grupoEconomicoId,
                idJornada,
                localidadesIds,
                cnaesIds,
                gruposClausulasIds,
                estabelecimentosIds,
                modulosSISAP,
                modulosComerciais
                );

            return Result.Success(usuario);
        }

        public Result Atualizar(string nome,
            Email email,
            string cargo,
            string? celular,
            string? ramal,
            int? idSuperior,
            string? departamento,
            bool bloqueado,
            bool documentoRestrito,
            Ausencia? ausencia,
            string? tipo,
            Nivel nivel,
            bool notificarWhatsapp,
            bool notificarEmail,
            int? grupoEconomicoId,
            int? idJornada,
            int[]? localidadesIds,
            int[]? cnaesIds,
            int[]? gruposClausulasIds,
            int[]? estabelecimentosIds,
            IEnumerable<UsuarioModulo>? modulosSISAP,
            IEnumerable<UsuarioModulo>? modulosComerciais)
        {
            if (nome is null) return Result.Failure<Usuario>("Informe o Nome");
            if (email is null) return Result.Failure<Usuario>("Informe o Email");
            if (cargo is null) return Result.Failure<Usuario>("Informe o Cargo");

            Nome = nome;
            Email = email;
            Cargo = cargo;
            Celular = celular;
            Ramal = ramal;
            IdSuperior = idSuperior;
            Departamento = departamento;
            Bloqueado = bloqueado;
            DocumentoRestrito = documentoRestrito;
            Ausencia = ausencia ?? Ausencia.SemAusencia();
            Tipo = tipo;
            Nivel = nivel;
            NotificarWhatsapp = notificarWhatsapp;
            NotificarEmail = notificarEmail;
            GrupoEconomicoId = grupoEconomicoId;
            JornadaId = idJornada;
            LocalidadesIds = localidadesIds;
            CnaesIds = cnaesIds;
            GruposClausulasIds = gruposClausulasIds;
            EstabelecimentosIds = estabelecimentosIds;
            _modulosComerciais = modulosComerciais?.ToList() ?? new List<UsuarioModulo>();
            _modulosSISAP = modulosSISAP?.ToList() ?? new List<UsuarioModulo>();

            return Result.Success();
        }

        internal bool TemGrupoEconomico()
        {
            return GrupoEconomicoId != null && GrupoEconomicoId > 0;
        }
    }
}