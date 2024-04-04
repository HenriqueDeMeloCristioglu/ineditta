namespace Ineditta.Integration.Auth.Dtos
{
    public sealed record FindUserDto
    {
        public string? Username { get; init; }
        public string? Email { get; init; }
        public bool Exact { get; init; }
    }
}
