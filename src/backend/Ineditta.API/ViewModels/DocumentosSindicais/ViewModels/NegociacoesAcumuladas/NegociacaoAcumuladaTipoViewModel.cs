namespace Ineditta.API.ViewModels.DocumentosSindicais.ViewModels.NegociacoesAcumuladas
{
    public class NegociacaoAcumuladaTipoViewModel
    {
        public int Ano { get; set; }
        public int Total { get; set; }
        public int AcordoColetivo { get; set; }
        public int AcordoColetivoEspecifico { get; set; }
        public int ConvencaoColetiva { get; set; }
        public int ConvencaoColetivaEspecifica { get; set; }
        public int TermoAditivoConvencaoColetiva { get; set; }
        public int TermoAditivoAcordoColetivo { get; set; }
        public DateTime MaiorDataAprovacao { get; set; }
    }
}
