using Cerebro.Core.IO;
using Cerebro.Core.Models.Entities.Addresses;
using Cerebro.Core.Models.Entities.Clients.Applications;
using LiteDB;

namespace Cerebro.Infrastructure.DataAccess.ApplicationStateStore
{
    public class ApplicationStateStoreDbContext
    {
        private ILiteDatabase db;

        public ApplicationStateStoreDbContext()
        {
            db = new LiteDatabase(ConfigLocations.GetApplicationStateStoreFile());

            InitializeCollections();
            EnsureKeys();
        }

        private void InitializeCollections()
        {
            Addresses = db.GetCollection<Address>("addresses", BsonAutoId.Int32);
            Applications = db.GetCollection<Application>("applications", BsonAutoId.Int32);
            ApplicationTokens = db.GetCollection<ApplicationToken>("application_tokens", BsonAutoId.Guid);
            ApplicationAddressConnections = db.GetCollection<ApplicationAddressConnection>("application_address_connections", BsonAutoId.Guid);

            ApplicationPermissions = db.GetCollection<ApplicationPermission>("application_permissions", BsonAutoId.Int32);
        }

        private void EnsureKeys()
        {
            Addresses!.EnsureIndex("address_unique_name_index", x => x.Name, unique: true);
            Applications!.EnsureIndex("application_unique_name_index", x => x.Name, unique: true);
            ApplicationTokens!.EnsureIndex("applicationtoken_applicationid_index", x => x.ApplicationId, unique: false);

            ApplicationPermissions!.EnsureIndex("applicationid_unique_index", x => x.ApplicationId, unique: true);
        }

        public ILiteCollection<Address>? Addresses { get; private set; }
        public ILiteCollection<Application>? Applications { get; private set; }
        public ILiteCollection<ApplicationToken>? ApplicationTokens { get; private set; }

        public ILiteCollection<ApplicationAddressConnection>? ApplicationAddressConnections { get; private set; }
        public ILiteCollection<ApplicationPermission>? ApplicationPermissions { get; private set; }
    }
}
