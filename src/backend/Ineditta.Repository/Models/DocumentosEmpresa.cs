using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class DocumentosEmpresa
{
    public int Id { get; set; }

    public string ClienteUnidadesIdUnidade { get; set; } = null!;

    public string DocumentosIdDocumento { get; set; } = null!;
}
