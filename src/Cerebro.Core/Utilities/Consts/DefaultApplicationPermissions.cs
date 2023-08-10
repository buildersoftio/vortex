using Cerebro.Core.Models.Entities.Clients.Applications;

namespace Cerebro.Core.Utilities.Consts
{
    public static class DefaultApplicationPermissions
    {
        public static ApplicationPermission CreateDefaultApplicationPermissionEntity(int applicationId, string createdBy)
        {

            var permissions = new Dictionary<string, string>
            {
                { READ_ADDRESS_PERMISSION_KEY, "*:{*}" },
                { WRITE_ADDRESS_PERMISSION_KEY, "*:{*}" },
                { CREATE_ADDRESS_PERMISSION_KEY, "True" }
            };

            return new ApplicationPermission()
            {
                ApplicationId = applicationId,
                CreatedAt = DateTime.Now,
                CreatedBy = createdBy,
                Permissions = permissions
            };
        }

        public const string READ_ADDRESS_PERMISSION_KEY = "READ_ADDRESSES";
        public const string WRITE_ADDRESS_PERMISSION_KEY = "WRITE_ADDRESSES";
        public const string CREATE_ADDRESS_PERMISSION_KEY = "CREATE_ADDRESSES";

    }
}
