using System;
using System.Collections.Generic;

using Ineditta.Application.Documentos.Sindicais.Entities;
using Ineditta.Application.Sindicatos.Laborais.Entities;

namespace Ineditta.Repository.Models;

public partial class DocSindSindEmp
{
    public int Id { get; set; }

    public int SindEmpIdSinde { get; set; }

    public int DocSindIdDoc { get; set; }

    public virtual DocumentoSindical DocSindIdDocNavigation { get; set; } = null!;

    public virtual SindicatoLaboral SindEmpIdSindeNavigation { get; set; } = null!;
}
