using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class NegociacaoPremissa
{
    public int IdnegociacaoPremissa { get; set; }

    public int Idnegociacao { get; set; }

    public string Premissa { get; set; } = null!;

    public string TipoPremissa { get; set; } = null!;

    public string Atual { get; set; } = null!;

    public string? Objetivo { get; set; }

    public string? Resultado { get; set; }

    public string? Aproveitamento { get; set; }

    public string? Comentários { get; set; }
}
