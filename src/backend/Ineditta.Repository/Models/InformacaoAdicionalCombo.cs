using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class InformacaoAdicionalCombo
{
    public int IdCombo { get; set; }

    public int AdTipoinformacaoadicionalId { get; set; }

    public string Options { get; set; } = null!;

    public virtual AdTipoinformacaoadicional AdTipoinformacaoadicional { get; set; } = null!;
}
