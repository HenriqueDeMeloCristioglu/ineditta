namespace Ineditta.API.ViewModels.EmailsCaixasDeSaida.ViewModels
{
    public class EmailCaixaDeSaidaDataTableViewModel
    {
        public string Email { get; set; } = null!;
        public string Assunto { get; set; } = null!;
        public DateOnly? DataInclusao { get; set; }
    }
}
