namespace Ineditta.API.ViewModels.Associacoes.ViewModels
{
    public class AssociacaoViewModel
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public int Id { get; set; }
        public string Sigla { get; set; }
        public string Cnpj { get; set; }
        public string Area { get; set; }
        public string Telefone { get; set; }
        public string Grau { get; set; }
        public string Grupo { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
