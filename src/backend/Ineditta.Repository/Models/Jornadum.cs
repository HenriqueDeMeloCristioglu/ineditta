using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class Jornadum
{
    public int IdJornada { get; set; }

    public string? JornadaSemanal { get; set; }

    public string? Descricao { get; set; }

    public sbyte IsDefault { get; set; }
}
