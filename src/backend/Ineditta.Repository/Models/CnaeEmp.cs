using System;
using System.Collections.Generic;

using Ineditta.Application.ClientesUnidades.Entities;
using Ineditta.Application.Cnaes.Entities;

namespace Ineditta.Repository.Models;

public partial class CnaeEmp
{
    public int IdcnaeEmp { get; set; }

    public DateOnly DataInicial { get; set; }

    public DateOnly? DataFinal { get; set; }

    public int ClasseCnaeIdclasseCnae { get; set; }

    public int ClienteUnidadesIdUnidade { get; set; }

    public virtual Cnae ClasseCnaeIdclasseCnaeNavigation { get; set; } = null!;

    public virtual ClienteUnidade ClienteUnidadesIdUnidadeNavigation { get; set; } = null!;
}
