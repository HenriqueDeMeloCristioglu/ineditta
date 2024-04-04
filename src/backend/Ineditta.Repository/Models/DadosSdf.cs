using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class DadosSdf
{
    public int IdFeriados { get; set; }

    public int FeriadosIdFeriado { get; set; }

    public int LocalizacaoIdLocalizacao { get; set; }

    public int? DocumentosIddocumentos { get; set; }

    public virtual Documento? DocumentosIddocumentosNavigation { get; set; }

    public virtual Feriado FeriadosIdFeriadoNavigation { get; set; } = null!;

    public virtual Application.Localizacoes.Entities.Localizacao LocalizacaoIdLocalizacaoNavigation { get; set; } = null!;
}
