using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class Feriado
{
    public int IdFeriado { get; set; }

    public string Dia { get; set; } = null!;

    public string Mes { get; set; } = null!;

    public string NomeFeriado { get; set; } = null!;

    public string Regra { get; set; } = null!;

    public string AbrangFeriado { get; set; } = null!;

    public int Localizacao { get; set; }

    public virtual ICollection<DadosSdf> DadosSdfs { get; set; } = new List<DadosSdf>();
}
