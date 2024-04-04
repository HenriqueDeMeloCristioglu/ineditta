using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class DocumentoAssunto
{
    public int IddocumentoAssunto { get; set; }

    public int DocumentosIddocumentos { get; set; }

    public int EstruturaClausulaIdEstruturaclausula { get; set; }

    public virtual Documento DocumentosIddocumentosNavigation { get; set; } = null!;
}
