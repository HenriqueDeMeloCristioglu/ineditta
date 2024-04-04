using System;
using System.Collections.Generic;

using Ineditta.Application.Clausulas.InformacoesAdicionais.Entities;

namespace Ineditta.Excel.ComparativosMapasSindicais.Dtos
{
    public class ComparativoMapaSindicalDto
    {
        public ComparativoMapaSindicalPrincipalVw Cabecalho { get; set; }
        public IEnumerable<ClausulaSindicalDadosDto> Dados { get; set; }
    }

    public IEnumerable<ClausulaSindicalCabecalhoDto> ListaCabecalhoFixo { get; set; }

    public class ClausulaSindicalCabecalhoDto
    { 
        public string Cabecalho { get; set; }
        public string Chave { get; set; }
        public string DescricaoChave { get; set; }
        public string Valor { get; set; }
    }

    public class ClausulaSindicalDadosDto
    {
        public string Cabecalho { get; set; }
        public string Grupo { get; set; }
        public string Descricao { get; set; }
        public TipoDado TipoDado { get; set; }
        public DateOnly? ValorData { get; set; }
        public decimal? ValorNumerico { get; set; }
        public string ValorTexto { get; set; }
        public decimal? ValorPercentual { get; set; }
        public string ValorDescricao { get; set; }
        public string ValorHora { get; set; }
        public string ValorCombo { get; set; }
    }
}
