using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class NegociacaoPautum
{
    public int IdnegociacaoPauta { get; set; }

    public int NegociacaoIdnegociacao { get; set; }

    public DateTime DataHora { get; set; }

    public string? Pauta { get; set; }
}
