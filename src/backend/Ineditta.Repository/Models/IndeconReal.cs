using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class IndeconReal
{
    public int Id { get; set; }

    public string Indicador { get; set; } = null!;

    public float DadoReal { get; set; }

    public DateTime CriadoEm { get; set; }

    public DateOnly? PeriodoData { get; set; }
}
