using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class Ninfoadicionai
{
    public int? IdClausulageralEstruturaClausula { get; set; }

    public int? DocSindIdDoc { get; set; }

    public int? ClausulaGeralIdClau { get; set; }

    public int? EstruturaClausulaIdEstruturaclausula { get; set; }

    public int? NomeInformacao { get; set; }

    public int? GrupoDados { get; set; }

    public int? AdTipoinformacaoadicionalCdtipoinformacaoadicional { get; set; }

    public int? IdInfoTipoGrupo { get; set; }

    public int? Sequencia { get; set; }

    public string? Texto { get; set; }

    public string? Numerico { get; set; }

    public string? Descricao { get; set; }

    public string? Data { get; set; }

    public string? Percentual { get; set; }

    public string? Hora { get; set; }

    public string? Combo { get; set; }
}
