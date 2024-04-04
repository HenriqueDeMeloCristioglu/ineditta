using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class InformacoesAdicionai
{
    public int IdinformacoesAdicionais { get; set; }

    public string? SindLaboral { get; set; }

    public string? CnpjSindlaboral { get; set; }

    public string? SindPatronal { get; set; }

    public string? CnpjSindpatronal { get; set; }

    public string? UfSindlaboral { get; set; }

    public string? DataAprovacao { get; set; }

    public string? NomeDocumento { get; set; }

    public string? Categoria { get; set; }

    public string? DataBase { get; set; }

    public string? ValidadeInicial { get; set; }

    public string? ValidadeFinal { get; set; }

    public string? GrupoClausula { get; set; }

    public string? NomeClausula { get; set; }

    public string? DocSindical { get; set; }

    public string? ClausulaGeral { get; set; }
}
