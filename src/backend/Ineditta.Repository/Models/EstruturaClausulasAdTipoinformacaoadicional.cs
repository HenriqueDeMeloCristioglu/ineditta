using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class EstruturaClausulasAdTipoinformacaoadicional
{
    public int IdEstruturaTagsClausulasAdTipoinformacaoadicional { get; set; }

    public int EstruturaClausulaIdEstruturaclausula { get; set; }

    public int AdTipoinformacaoadicionalCdtipoinformacaoadicional { get; set; }

    public virtual AdTipoinformacaoadicional AdTipoinformacaoadicionalCdtipoinformacaoadicionalNavigation { get; set; } = null!;
}
