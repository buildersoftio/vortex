namespace Cerebro.Core.Models.Dtos.Applications
{
    public class ApplicationPermissionDto
    {
        public string ApplicationName { get; set; }
        public Dictionary<string, string>? Permissions { get; set; }
    }
}
