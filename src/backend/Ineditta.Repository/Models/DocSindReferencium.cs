using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class DocSindReferencium
{
    public int IddocSindReferencia { get; set; }

    public int DocSindIdDoc { get; set; }

    public int EstruturaClausulaIdEstruturaclausula { get; set; }
}
