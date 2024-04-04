using System;
using System.Collections.Generic;

using Ineditta.Application.Cnaes.Entities;
using Ineditta.Application.Sindicatos.Patronais.Entities;

namespace Ineditta.Repository.Models;

public partial class BaseTerritorialsindpatro
{
    public int IdbaseTerritorialSindPatro { get; set; }

    public int SindPatronalIdSindp { get; set; }

    public int LocalizacaoIdLocalizacao1 { get; set; }

    public int ClasseCnaeIdclasseCnae { get; set; }

    public DateOnly DataInicial { get; set; }

    public DateOnly? DataFinal { get; set; }

    public virtual Cnae ClasseCnaeIdclasseCnaeNavigation { get; set; } = null!;

    public virtual Application.Localizacoes.Entities.Localizacao LocalizacaoIdLocalizacao1Navigation { get; set; } = null!;

    public virtual SindicatoPatronal SindPatronalIdSindpNavigation { get; set; } = null!;
}
