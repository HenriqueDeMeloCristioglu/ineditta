using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class InformacaoAdicionalGrupo
{
    public int IdGrupo { get; set; }

    public int AdTipoinformacaoadicionalId { get; set; }

    public int InformacaoadicionalNoGrupo { get; set; }

    public int Sequencia { get; set; }
    
    public bool ExibeComparativoMapaSindical { get; set; }

    public virtual AdTipoinformacaoadicional AdTipoinformacaoadicional { get; set; } = null!;

    public virtual AdTipoinformacaoadicional InformacaoadicionalNoGrupoNavigation { get; set; } = null!;
}
