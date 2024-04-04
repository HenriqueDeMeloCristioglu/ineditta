using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class WhCalendario
{
    public int IdcalendarGeral { get; set; }

    public int UsuarioAdmIdUser { get; set; }

    public string Origem { get; set; } = null!;

    public string? NomeGrupoclausula { get; set; }

    public string? NomeClausula { get; set; }

    public int? IdClausula { get; set; }

    public string NomeEvento { get; set; } = null!;

    public DateOnly DataInicial { get; set; }

    public DateOnly? DataFinal { get; set; }

    public TimeOnly? HoraEvento { get; set; }

    public string? Abrangencia { get; set; }

    public int? AdTipoinformacaoadicionalCdtipoinformacaoadicional { get; set; }

    public string? Dados { get; set; }

    public string? IdSinde { get; set; }

    public string? IdSindp { get; set; }

    public string? Comentario { get; set; }

    public string? IdCnaes { get; set; }
}
