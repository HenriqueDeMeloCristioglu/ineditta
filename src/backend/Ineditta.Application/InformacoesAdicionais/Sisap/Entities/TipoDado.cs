using System.ComponentModel;

namespace Ineditta.Application.Clausulas.Entities.InformacoesAdicionais
{
    public enum TipoDado
    {
        [Description("C")]
        Combo,
        [Description("D")]
        Data,
        [Description("L")]
        Percentual,
        [Description("V")]
        Monetario,
        [Description("P")]
        Descricao,
        [Description("N")]
        Inteiro,
        [Description("H")]
        Hora,
        [Description("CM")]
        ComboMultipla,
        [Description("T")]
        Texto
    }
}
