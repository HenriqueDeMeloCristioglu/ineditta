namespace Ineditta.API.ViewModels.CentraisSindicais.ViewModels
{
    public class CentralSindicalViewModel
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public int Id { get; set; }
        public string Sigla { get; set; }
        public string Cnpj { get; set; }
        public string Telefone { get; set; }
        public string Nome { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
