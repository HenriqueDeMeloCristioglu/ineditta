using CSharpFunctionalExtensions;

using Ineditta.Application.Usuarios.Entities;
using Ineditta.Application.UsuariosTiposEventosCalendarioSindical.Entities;

using MediatR;

namespace Ineditta.Application.Usuarios.UseCases.Upsert
{
    public class UpsertUsuarioRequest : IRequest<Result>
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Cargo { get; set; }
        public string? Celular { get; set; }
        public string? Ramal { get; set; }
        public int? SuperiorId { get; set; }
        public string? Departamento { get; set; }
        public string? Foto { get; set; }
        public bool Bloqueado { get; set; }
        public bool DocumentoRestrito { get; set; }
        public DateOnly? AusenciaInicio { get; set; }
        public DateOnly? AusenciaFim { get; set; }
        public string? Tipo { get; set; }
        public Nivel Nivel { get; set; }
        public bool NotificarWhatsapp { get; set; }
        public bool NotificarEmail { get; set; }
        public int? GrupoEconomicoId { get; set; }
        public int? JornadaId { get; set; }
        public int[]? LocalidadesIds { get; set; }
        public int[]? CnaesIds { get; set; }
        public int[]? GruposClausulasIds { get; set; }
        public int[]? EstabelecimentosIds { get; set; }
        public bool TrocaSenhaLogin { get; set; }
        public IEnumerable<UpsertUsuarioModuloRequest>? ModulosSisap { get; set; }
        public IEnumerable<UpsertUsuarioModuloRequest>? ModulosComerciais { get; set; }
        public IEnumerable<UsuarioTipoEventoCalendarioSindicalInputModel>? CalendarioConfig { get; set; }
    }

    public class UpsertUsuarioModuloRequest
    {
        public long Id { get; set; }
        public bool Criar { get; set; }
        public bool Consultar { get; set; }
        public bool Comentar { get; set; }
        public bool Alterar { get; set; }
        public bool Excluir { get; set; }
        public bool Aprovar { get; set; }
    }

    public class UsuarioTipoEventoCalendarioSindicalInputModel
    {
        public long? Id { get; set; }
        public int UsuarioId { get; set; }
        public int TipoId { get; set; }
        public int? SubtipoId { get; set; }
        public bool NotificarEmail { get; set; }
        public bool NotificarWhatsapp { get; set; }
        public int? NotificarAntes { get; set; }
    }
}