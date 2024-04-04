namespace Ineditta.API.ViewModels.Federacoes.ViewModels
{
    public class FederacaoViewModel
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public int Id { get; set; }
        public string Sigla { get; set; }
        public string CNPJ { get; set; }
        public string AreaGeoeconomica { get; set; }
        public string Telefone { get; set; }
        public string Grupo { get; set; }
        public string Grau { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
