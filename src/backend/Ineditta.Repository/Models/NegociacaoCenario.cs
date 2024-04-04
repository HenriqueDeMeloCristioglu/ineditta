using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class NegociacaoCenario
{
    public int IdnegociacaoPremissas { get; set; }

    public int NegociacaoIdnegociacao { get; set; }

    public int EstruturaClausulaIdEstruturaclausula { get; set; }

    public int? AdTipoinformacaoadicionalCdtipoinformacaoadicional { get; set; }

    public int? IdInfoTipoGrupo { get; set; }

    public int? Sequencia { get; set; }

    public float? Numerico { get; set; }

    public float? Percentual { get; set; }

    public string? Combo { get; set; }
}
