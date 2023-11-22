namespace Vortex.Core.Models.Configurations
{
    public class CredentialsConfiguration
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
    }

    public enum UserRole
    {
        Admin,
        Readonly
    }
}
