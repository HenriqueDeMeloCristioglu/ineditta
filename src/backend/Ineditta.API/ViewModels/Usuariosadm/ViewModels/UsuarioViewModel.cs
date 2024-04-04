using System.Text.Json.Serialization;

using Ineditta.API.ViewModels.Shared.ViewModels;
using Ineditta.Application.Usuarios.Entities;

namespace Ineditta.API.ViewModels.Usuariosadm.ViewModels
{
    public class UsuarioViewModel
    {
        public long Id { get; set; }
        public string? Nome { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Cargo { get; set; }
        public string? Celular { get; set; }
        public string? Ramal { get; set; }
        public OptionModel<long>? Superior { get; set; }
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
        public OptionModel<long>? GrupoEconomico { get; set; }
        public OptionModel<long>? Jornada { get; set; }
        public IEnumerable<OptionModel<int>>? Localidades { get; set; }
        public IEnumerable<OptionModel<int>>? Cnaes { get; set; }
        public IEnumerable<OptionModel<int>>? GruposClausulas { get; set; }
        public int[]? EstabelecimentosIds { get; set; }
        public bool TrocaSenhaLogin { get; set; }
        [JsonIgnore]
        public int[]? LocalidadesIds { get; set; }
        [JsonIgnore]
        public int[]? CnaesIds { get; set; }
        [JsonIgnore]
        public int[]? GruposClausulasIds { get; set; }
        [JsonIgnore]
        public IEnumerable<UsuarioModulo>? UsuarioModulosSisap { get; set; }
        [JsonIgnore]
        public IEnumerable<UsuarioModulo>? UsuarioModulosComerciais { get; set; }
        public IEnumerable<UsuarioModuloViewModel>? ModulosSisap => UsuarioModulosSisap == null ? default : UsuarioModulosSisap!.Select(ms => new UsuarioModuloViewModel
        {
            Id = ms.Id,
            Alterar = ms.Alterar,
            Aprovar = ms.Aprovar,
            Comentar = ms.Comentar,
            Consultar = ms.Consultar,
            Criar = ms.Criar,
            Excluir = ms.Excluir
        });

        public IEnumerable<UsuarioModuloViewModel>? ModulosComerciais => UsuarioModulosComerciais == null ? default : UsuarioModulosComerciais!.Select(ms => new UsuarioModuloViewModel
        {
            Id = ms.Id,
            Alterar = ms.Alterar,
            Aprovar = ms.Aprovar,
            Comentar = ms.Comentar,
            Consultar = ms.Consultar,
            Criar = ms.Criar,
            Excluir = ms.Excluir
        });

        public DateTime? DataCriacao { get; set; }
        public string? NomeUserCriador { get; set; }
        public IEnumerable<OptionModel<int>>? GruposEconomicosConsultores { get; set; }
        public IEnumerable<UsuarioTipoEventoCalendarioSindicalViewModel>? UsuariosTiposEventosCalendario { get; set; }
    }

    public class UsuarioModuloViewModel
    {
        public long Id { get; set; }
        public bool Criar { get; set; }
        public bool Consultar { get; set; }
        public bool Comentar { get; set; }
        public bool Alterar { get; set; }
        public bool Excluir { get; set; }
        public bool Aprovar { get; set; }
    }

    public class UsuarioTipoEventoCalendarioSindicalViewModel
    {
        public long Id { get; set; }
        public long UsuarioId { get; set; }
        public int Tipo { get; set; }
        public int? Subtipo { get; set; }
        public bool NotificarEmail { get; set; }
        public bool NotificarWhatsapp { get; set; }
        public int NotificarAntes { get; set; }
    }
}
