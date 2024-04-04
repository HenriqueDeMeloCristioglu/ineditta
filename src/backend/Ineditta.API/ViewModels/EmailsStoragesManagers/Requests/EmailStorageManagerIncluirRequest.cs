namespace Ineditta.API.ViewModels.EmailsStoragesManagers.Requests
{
    public class EmailStorageManagerIncluirRequest
    {
        public string Email { get; set; } = null!;
        public int StatusMessage { get; set; }
        public string Assunto { get; set; } = null!;
    }
}
