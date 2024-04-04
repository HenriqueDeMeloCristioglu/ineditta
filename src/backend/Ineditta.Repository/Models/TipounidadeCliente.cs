using System;
using System.Collections.Generic;

using Ineditta.Application.ClientesUnidades.Entities;

namespace Ineditta.Repository.Models;

public partial class TipounidadeCliente
{
    public int IdTiponegocio { get; set; }

    public string TipoNegocio { get; set; } = null!;

    public virtual ICollection<ClienteUnidade> ClienteUnidades { get; set; } = new List<ClienteUnidade>();
}
