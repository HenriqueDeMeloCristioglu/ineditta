namespace Ineditta.Application.SharedKernel.Auth
{
    public record UsuarioAuthDto
    {
        public Guid Id { get; init; }
        public string? Email { get; init; }
        public string? Username { get; init; }
    }
}
