using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class DocumentosAbrangencium
{
    public int IdAbrangDoc { get; set; }

    public int LocalizacaoIdLocalizacao { get; set; }

    public int DocumentosIddocumentos { get; set; }

    public virtual Documento DocumentosIddocumentosNavigation { get; set; } = null!;

    public virtual Application.Localizacoes.Entities.Localizacao LocalizacaoIdLocalizacaoNavigation { get; set; } = null!;
}
