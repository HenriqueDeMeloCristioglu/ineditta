using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class FormularioGrupo
{
    public int IdFormulariogrupo { get; set; }

    public int DocSindIdDoc { get; set; }

    public int ClienteGrupoIdGrupoEconomico { get; set; }

    public string? FormularioComunicado { get; set; }
}
