using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class Atividade
{
    public int IdAtividades { get; set; }

    public int TarefasIdTarefas { get; set; }

    public int UsuarioAdmIdUser { get; set; }

    public string? NomeAtividade { get; set; }

    public string? StatusEtapa { get; set; }

    public DateOnly DataAbertura { get; set; }

    public DateOnly DataInicial { get; set; }

    public DateOnly DataFinal { get; set; }

    public DateOnly DataEvento { get; set; }

    public string? Alerta { get; set; }

    public string? Recorrencia { get; set; }

    public string? CaminhoDocumentos { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<AtividadesComentario> AtividadesComentarios { get; set; } = new List<AtividadesComentario>();

    public virtual Tarefa TarefasIdTarefasNavigation { get; set; } = null!;
}
