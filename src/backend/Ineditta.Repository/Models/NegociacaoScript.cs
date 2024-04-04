using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class NegociacaoScript
{
    public int IdnegociacaoScript { get; set; }

    public int NegociacaoIdnegociacao { get; set; }

    public DateOnly DataScript { get; set; }

    public string TextoScript { get; set; } = null!;
}
