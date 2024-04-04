using System;
using System.Collections.Generic;

using Ineditta.Application.Documentos.Sindicais.Entities;

namespace Ineditta.Repository.Models;

public partial class AbrangDocsind
{
    public int IdAbrgdocsind { get; set; }

    public int LocalizacaoIdLocalizacao { get; set; }

    public int DocSindIdDocumento { get; set; }

    public virtual DocumentoSindical DocSindIdDocumentoNavigation { get; set; } = null!;

    public virtual Application.Localizacoes.Entities.Localizacao LocalizacaoIdLocalizacaoNavigation { get; set; } = null!;
}
