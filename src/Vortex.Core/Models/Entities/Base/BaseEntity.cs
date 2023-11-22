namespace Vortex.Core.Models.Entities.Base
{
    public class BaseEntity
    {

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }


        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }
        public string? CreatedBy { get; set; }

        public BaseEntity()
        {
            IsActive = true;
            IsDeleted = false;

            CreatedAt = DateTimeOffset.UtcNow;
        }
    }
}
