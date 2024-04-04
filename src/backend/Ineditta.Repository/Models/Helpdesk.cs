using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class Helpdesk
{
    public int Idhelpdesk { get; set; }

    public int IdUserC { get; set; }

    public int? IdUserR { get; set; }

    public string? ComentarioChamado { get; set; }

    public DateTime? DataAbertura { get; set; }

    public DateOnly? DataVencimento { get; set; }

    public DateTime? DataFechado { get; set; }

    public string? TipoChamado { get; set; }

    public int? Estabelecimento { get; set; }

    public int? SindLaboral { get; set; }

    public int? SindPatronal { get; set; }

    public string? StatusChamado { get; set; }

    public int? Clausula { get; set; }

    public DateTime? InicioResposta { get; set; }

    public string? CaminhoArquivo { get; set; }
}
