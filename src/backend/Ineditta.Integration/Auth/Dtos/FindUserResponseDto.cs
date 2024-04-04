namespace Ineditta.Integration.Auth.Dtos
{
    public class FindUserResponseDto
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
    }
}
