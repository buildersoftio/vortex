using Cerebro.Core.Models.Entities.Clients.Applications;

namespace Cerebro.Core.Utilities.Consts
{
    public static class DefaultApplicationPermissions
    {
        public static ApplicationPermission CreateDefaultApplicationPermissionEntity(int applicationId, string createdBy)
        {

            var permissions = new Dictionary<string, string>
            {
                { "READ_ADDRESSES", "*:{*}" },
                { "WRITE_ADDRESSES", "*:{*}" },
                { "CREATE_ADDRESSES", "True" }
            };

            return new ApplicationPermission()
            {
                ApplicationId = applicationId,
                CreatedAt = DateTime.Now,
                CreatedBy = createdBy,
                Permissions = permissions
            };
        }

    }
}
