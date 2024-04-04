using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class DocumentosLocalizado
{
    public int IdDocumento { get; set; }

    public string NomeDocumento { get; set; } = null!;

    public string? Origem { get; set; }

    public string Caminho { get; set; } = null!;

    public DateTime DataRegistro { get; set; }

    public string? Situacao { get; set; }

    public DateOnly? DataAprovacao { get; set; }

    public string Referenciado { get; set; } = null!;

    public int? IdLegado { get; set; }

    public string? Uf { get; set; }
}
