using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class NegociacaoRodadum
{
    public int IdnegociacaoRodada { get; set; }

    public int NegociacaoIdnegociacao { get; set; }

    public string NumeroRodada { get; set; } = null!;

    public DateOnly DataRodada { get; set; }

    public int FaseCctIdFase { get; set; }

    public string? Aproveitamento { get; set; }
}
