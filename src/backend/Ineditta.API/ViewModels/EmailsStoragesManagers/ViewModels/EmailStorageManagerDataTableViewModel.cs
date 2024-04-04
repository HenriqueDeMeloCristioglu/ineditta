namespace Ineditta.API.ViewModels.EmailsStoragesManagers.ViewModels
{
    public class EmailStorageManagerDataTableViewModel
    {
        public string To { get; set; } = null!;
        public string From { get; set; } = null!;
        public string Assunto { get; set; } = null!;
        public string Enviado { get; set; } = null!;
        public DateTime DataInclusao { get; set; }
    }
}
