using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

/// <summary>
/// Tabela contendo informações de Cláusulas importadas do Banco de dados do Sistema Legado
/// </summary>
public partial class BdLegadoClausula
{
    public int Id { get; set; }

    public int CodigoInternoSindicato { get; set; }

    public string NomeDoSindicato { get; set; } = null!;

    public string CnpjDoSindicato { get; set; } = null!;

    public string CategoriaSindical { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public string Cidade { get; set; } = null!;

    public int CodigoInternoClausula { get; set; }

    public string NomeDaClausula { get; set; } = null!;

    public string Grupo { get; set; } = null!;

    public string Documento { get; set; } = null!;

    public DateOnly PeriodoInicial { get; set; }

    public DateOnly PeriodoFinal { get; set; }

    public string Database { get; set; } = null!;

    public string TextoClausula { get; set; } = null!;

    public DateOnly? DataDeInclusao { get; set; }
}
