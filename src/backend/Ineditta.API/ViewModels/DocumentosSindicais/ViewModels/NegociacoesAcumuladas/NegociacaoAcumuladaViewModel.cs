namespace Ineditta.API.ViewModels.DocumentosSindicais.ViewModels.NegociacoesAcumuladas
{
    public class NegociacaoAcumuladaViewModel
    {
        public int Ano { get; set; }
        public int Total { get; set; }
        public DateTime? MaiorDataAprovacao { get; set; }
        public IEnumerable<NegociacaoAcumuladaItemViewModel> Itens { get; set; } = null!;

        public static implicit operator NegociacaoAcumuladaViewModel(NegociacaoAcumuladaTipoViewModel model)
        {
            return model is null
                ? new NegociacaoAcumuladaViewModel()
                : new NegociacaoAcumuladaViewModel
                {
                    Ano = model.Ano,
                    Total = model.Total,
                    MaiorDataAprovacao = model.MaiorDataAprovacao,
                    Itens = new List<NegociacaoAcumuladaItemViewModel>
                {
                    new NegociacaoAcumuladaItemViewModel
                    {
                        Id = 1,
                        NomeDocumento = "Acordos coletivos",
                        Quantidade = model.AcordoColetivo,
                        Proporcao = Math.Round( 100m * model.AcordoColetivo / model.Total, 4)
                    },
                    new NegociacaoAcumuladaItemViewModel
                    {
                        Id = 2,
                        NomeDocumento = "Acordos Coletivos Específicos",
                        Quantidade = model.AcordoColetivoEspecifico,
                        Proporcao = Math.Round( 100m * model.AcordoColetivoEspecifico / model.Total, 4)
                    },
                    new NegociacaoAcumuladaItemViewModel
                    {
                        Id = 3,
                        NomeDocumento = "Convenções Coletivas",
                        Quantidade = model.ConvencaoColetiva,
                        Proporcao = Math.Round( 100m * model.ConvencaoColetiva / model.Total, 4)
                    },
                    new NegociacaoAcumuladaItemViewModel
                    {
                        Id = 4,
                        NomeDocumento = "Convenções Coletivas Específicas",
                        Quantidade = model.ConvencaoColetivaEspecifica,
                        Proporcao = Math.Round( 100m * model.ConvencaoColetivaEspecifica / model.Total, 4)
                    },
                    new NegociacaoAcumuladaItemViewModel
                    {
                        Id = 5,
                        NomeDocumento = "Termo Aditivo Convenção Coletiva",
                        Quantidade = model.TermoAditivoConvencaoColetiva,
                        Proporcao = Math.Round( 100m * model.TermoAditivoConvencaoColetiva / model.Total, 4)
                    },
                    new NegociacaoAcumuladaItemViewModel
                    {
                        Id = 6,
                        NomeDocumento = "Termo Aditivo Acordo Coletivo",
                        Quantidade = model.TermoAditivoAcordoColetivo,
                        Proporcao = Math.Round( 100m * model.TermoAditivoAcordoColetivo / model.Total, 4)
                    }
                }
                };
        }
    }
    public class NegociacaoAcumuladaItemViewModel
    {
        public int Id { get; set; }
        public string NomeDocumento { get; set; } = null!;
        public int Quantidade { get; set; }
        public decimal Proporcao { get; set; }
    }
}
