using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class Indecon
{
    public int IdIndecon { get; set; }

    public string Indicador { get; set; } = null!;

    public string Origem { get; set; } = null!;

    public string? Fonte { get; set; }

    public string Data { get; set; } = null!;

    public float? DadoProjetado { get; set; }

    public float? DadoReal { get; set; }

    public int? ClienteGrupoIdGrupoEconomico { get; set; }

    public DateTime? CriadoEm { get; set; }

    public int IdUsuario { get; set; }
}
