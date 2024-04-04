namespace Ineditta.Application.Usuarios.Dtos
{
    public record EmailBoaVindaDto
    {
        public EmailBoaVindaDto(string url, string email, string username, string authUsername)
        {
            Url = url;
            Email = email;
            Username = username;
            Senha = "Ineditta123@";
            AuthUsername = authUsername;
        }

        public string Url { get; init; }
        public string Email { get; init; }
        public string Username { get; init; }
        public string Senha { get; init; }
        public string AuthUsername { get; init; }
    }
}
