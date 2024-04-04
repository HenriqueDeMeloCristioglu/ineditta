using System;
using System.Collections.Generic;

using Ineditta.Application.Sindicatos.Patronais.Entities;

namespace Ineditta.Repository.Models;

public partial class SindDirpatro
{
    public int IdDiretoriap { get; set; }

    public string DirigenteP { get; set; } = null!;

    public DateOnly InicioMandatop { get; set; }

    public DateOnly? TerminoMandatop { get; set; }

    public string FuncaoP { get; set; } = null!;

    public int SindPatrIdSindp { get; set; }

    public int? ClienteMatrizIdEmpresa { get; set; }

    public string? SituacaoP { get; set; }

    public int? ClienteUnidadesIdUnidade { get; set; }

    public virtual SindicatoPatronal SindPatrIdSindpNavigation { get; set; } = null!;
}
