using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class Tarefa
{
    public int IdTarefas { get; set; }

    public int CalendarGeralIdcalendarGeral { get; set; }

    public int? UsuarioAdmIdUser { get; set; }

    public string? StatusTarefa { get; set; }

    public DateOnly DataAbertura { get; set; }

    public DateOnly DataInicial { get; set; }

    public DateOnly? DataFinal { get; set; }

    public DateOnly DataEvento { get; set; }

    public string NomeTarefa { get; set; } = null!;

    public int? Assunto { get; set; }

    public virtual ICollection<Atividade> Atividades { get; set; } = new List<Atividade>();
}
