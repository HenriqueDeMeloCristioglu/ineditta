using System.ComponentModel;

namespace Ineditta.Application.AIs.DocumentosSindicais.Entities
{
    public enum IADocumentoStatus
    {
        [Description("Aguardando processamento")]
        AguardandoProcessamento = 1,
        [Description("Quebrando cláusulas")]
        QuebrandoClausulas = 2,
        [Description("Aguardando aprovação da quebra de cláusulas")]
        AguardandoAprovacaoQuebraClausula = 3,
        [Description("Classificando cláusulas")]
        ClassificandoClausulas = 4,
        [Description("Aguardando aprovação da classificação")]
        AguardandoAprovacaoClassificacao = 5,
        [Description("Aprovado")]
        Aprovado = 6,
        [Description("Erro")]
        Erro = 7
    }
}