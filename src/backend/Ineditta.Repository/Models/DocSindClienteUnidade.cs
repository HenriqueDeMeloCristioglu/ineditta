using System;
using System.Collections.Generic;

using Ineditta.Application.ClientesUnidades.Entities;
using Ineditta.Application.Documentos.Sindicais.Entities;

namespace Ineditta.Repository.Models;

public partial class DocSindClienteUnidade
{
    public int IdDocsindClienteunidades { get; set; }

    public int DocSindIdDoc { get; set; }

    public int ClienteUnidadesIdUnidade { get; set; }

    public virtual ClienteUnidade ClienteUnidadesIdUnidadeNavigation { get; set; } = null!;

    public virtual DocumentoSindical DocSindIdDocNavigation { get; set; } = null!;
}
