using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class AtividadesComentario
{
    public int IdatividadesComentarios { get; set; }

    public int AtividadesIdAtividades { get; set; }

    public string Comentario { get; set; } = null!;

    public DateOnly DataComentario { get; set; }

    public virtual Atividade AtividadesIdAtividadesNavigation { get; set; } = null!;
}
