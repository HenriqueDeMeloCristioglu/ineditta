using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class FiltroCsv
{
    public int Id { get; set; }

    public string Filtro { get; set; } = null!;

    public string? Usuario { get; set; }
}
