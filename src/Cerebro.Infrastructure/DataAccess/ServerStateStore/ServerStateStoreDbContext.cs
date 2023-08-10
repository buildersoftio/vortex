using Cerebro.Core.IO;
using Cerebro.Core.Models.Entities.Addresses;
using Cerebro.Core.Models.Entities.Clients.Applications;
using LiteDB;

namespace Cerebro.Infrastructure.DataAccess.ServerStateStore
{
    public class ServerStateStoreDbContext
    {
        private ILiteDatabase db;

        public ServerStateStoreDbContext()
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
            ClientConnections = db.GetCollection<ClientConnection>("client_connections", BsonAutoId.Guid);

            ApplicationPermissions = db.GetCollection<ApplicationPermission>("application_permissions", BsonAutoId.Int32);
        }

        private void EnsureKeys()
        {
            Addresses!.EnsureIndex(x => x.Name, unique: true);
            Addresses!.EnsureIndex(x => x.Alias, unique: true);

            Applications!.EnsureIndex(x => x.Name, unique: true);
            ApplicationTokens!.EnsureIndex(x => x.ApplicationId, unique: false);

            ApplicationPermissions!.EnsureIndex(x => x.ApplicationId, unique: true);
        }

        public ILiteCollection<Address>? Addresses { get; private set; }

        public ILiteCollection<Application>? Applications { get; private set; }
        public ILiteCollection<ApplicationToken>? ApplicationTokens { get; private set; }

        public ILiteCollection<ClientConnection>? ClientConnections { get; private set; }
        public ILiteCollection<ApplicationPermission>? ApplicationPermissions { get; private set; }
    }
}
