using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class DiretoriaSindLaboral
{
    public int IdDiretoriae { get; set; }

    public string DirigenteE { get; set; } = null!;

    public DateOnly InicioMandatoe { get; set; }

    public DateOnly? TerminoMandatoe { get; set; }

    public string FuncaoE { get; set; } = null!;

    public int SindEmpIdSinde { get; set; }

    public int? ClienteMatrizIdEmpresa { get; set; }

    public string? SituacaoE { get; set; }

    public int? ClienteUnidadesIdUnidade { get; set; }
}
