using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class FormularioFolha
{
    public int IdformularioFolha { get; set; }

    public int? DocSind { get; set; }

    public int? Usuario { get; set; }

    public DateOnly? DataAprovacao { get; set; }

    public int? NomeDocumento { get; set; }

    public int? AtividadeEconomica { get; set; }

    public string? CodSindcliente { get; set; }

    public int? SiglaLaboral { get; set; }

    public int? SiglaPatronal { get; set; }

    public string? DataBase { get; set; }

    public DateOnly? ValidadeFinal { get; set; }

    public DateOnly? UltimaAtualizacao { get; set; }

    public string? Status { get; set; }

    public string? Formulario { get; set; }

    public string? Caminho { get; set; }

    public string? NomeArqExcell { get; set; }
}
