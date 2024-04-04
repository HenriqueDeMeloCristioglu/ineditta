using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class AnuenciaInicial
{
    public int IdanuenciaInicial { get; set; }

    public string UsuarioAdmEmail { get; set; } = null!;

    public DateTime DataAnuencia { get; set; }
}
