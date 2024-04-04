using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class AdTipoinformacaoadicional
{
    public int Cdtipoinformacaoadicional { get; set; }

    public string? Nmtipoinformacaoadicional { get; set; }

    public string? Idtipodado { get; set; }

    public DateOnly? Dtultatualizacao { get; set; }

    public string? ClasseIa { get; set; }

    public string? TarefaInf { get; set; }

    public string? UsoDado { get; set; }

    public string? DadoMs { get; set; }

    public string? SorthName { get; set; }

    public string? Calendario { get; set; }

    public string? ComboOptions { get; set; }

    public virtual ICollection<EstruturaClausulasAdTipoinformacaoadicional> EstruturaClausulasAdTipoinformacaoadicionals { get; set; } = new List<EstruturaClausulasAdTipoinformacaoadicional>();

    public virtual ICollection<InformacaoAdicionalCombo> InformacaoAdicionalCombos { get; set; } = new List<InformacaoAdicionalCombo>();

    public virtual ICollection<InformacaoAdicionalGrupo> InformacaoAdicionalGrupoAdTipoinformacaoadicionals { get; set; } = new List<InformacaoAdicionalGrupo>();

    public virtual ICollection<InformacaoAdicionalGrupo> InformacaoAdicionalGrupoInformacaoadicionalNoGrupoNavigations { get; set; } = new List<InformacaoAdicionalGrupo>();
}
