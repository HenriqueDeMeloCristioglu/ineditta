namespace Ineditta.Integration.Auth.Dtos
{
    public class CredentialsDto
    {
        public required string Type { get; set; }
        public required string Value { get; set; }
        public bool Temporary { get; set; }
    }

    public class AccessDto
    {
        public bool ManageGroupMembership { get; set; }
        public bool View { get; set; }
        public bool MapRoles { get; set; }
        public bool Impersonate { get; set; }
        public bool Manage { get; set; }
    }

    public class AttributesDto
    {
        public string? IdFilial { get; set; }
        public string? Tipo { get; set; }
    }

    public class CreateUserDto
    {
        public long CreatedTimestamp { get; set; }
        public required string Username { get; set; }
        public bool Enabled { get; set; }
        public bool Totp { get; set; }
        public bool EmailVerified { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public List<CredentialsDto>? Credentials { get; set; }
        public List<object>? DisableableCredentialTypes { get; set; }
        public List<object>? RequiredActions { get; set; }
        public int NotBefore { get; set; }
        public AccessDto? Access { get; set; }
        public List<object>? RealmRoles { get; set; }
        public AttributesDto? Attributes { get; set; }
    }
}
