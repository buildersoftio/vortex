using Cerebro.Core.Models.Common;
using Cerebro.Core.Models.Entities.Clients.Applications;

namespace Cerebro.Core.Models.Dtos.Applications
{
    public class ApplicationTokenDto
    {
        public Guid Id { get; set; }
        public string ApplicationName { get; set; }
        public CryptographyTypes CryptographyType { get; set; }
        public bool IsActive { get; set; }

        public DateTimeOffset ExpireDate { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset IssuedDate { get; set; }

        public ApplicationTokenDto()
        {
            
        }

        public ApplicationTokenDto(Guid id, string applicationName, CryptographyTypes cryptographyType, bool isActive, DateTimeOffset expireDate, string? description, DateTimeOffset issuedDate)
        {
            Id = id;
            ApplicationName = applicationName;
            CryptographyType = cryptographyType;
            IsActive = isActive;
            ExpireDate = expireDate;
            Description = description;
            IssuedDate = issuedDate;
        }
    }

    public class TokenRequest
    {
        public CryptographyTypes CryptographyType { get; set; }
        public DateTimeOffset ExpireDate { get; set; }
        public string? Description { get; set; }
    }

    public class TokenResponse
    {
        public string ApplicationName { get; set; }

        public Guid Key { get; set; }
        public string Secret { get; set; }

        public DateTimeOffset ExpireDate { get; set; }
    }
}
