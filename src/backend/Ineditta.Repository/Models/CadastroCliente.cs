using System;
using System.Collections.Generic;

namespace Ineditta.Repository.Models;

public partial class CadastroCliente
{
    public int IdcadClientes { get; set; }

    public string CadSuperior { get; set; } = null!;

    public string TipoCliente { get; set; } = null!;

    public string? NomeCliente { get; set; }

    public string? LogoGrupo { get; set; }

    public string? GeSlaentrega { get; set; }

    public string? GeSlaprioridade { get; set; }

    public string? GeDatafopag { get; set; }

    public string? GeTipoprocessamento { get; set; }

    public string? GeTipodocumento { get; set; }

    public string? EpNomeempresa { get; set; }

    public string? EpCodigoempresa { get; set; }

    public string? EstNomeunidade { get; set; }

    public string? EstCodunidade { get; set; }

    public string? EstCnpj { get; set; }

    public string? EstLogradouro { get; set; }

    public string? EstBairro { get; set; }

    public string? EstRegional { get; set; }

    public string? EstCep { get; set; }

    public DateOnly EstDatainclusao { get; set; }

    public DateOnly? EstDatainativacao { get; set; }

    public string? EstCodsindlaboral { get; set; }

    public string? EstCodsindpatronal { get; set; }

    public string? EstTipounidcliente { get; set; }

    public int EstLocalizacao { get; set; }

    public string EstCnaemp { get; set; } = null!;

    public string Modulos { get; set; } = null!;
}
