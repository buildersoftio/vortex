using Vortex.Core.IO;
using Vortex.Core.Models.Entities.Entries;
using LiteDB;

namespace Vortex.Infrastructure.DataAccess.IndexesState
{
    public class IndexCatalogDbContext
    {
        private ILiteDatabase db;

        public IndexCatalogDbContext()
        {
            db = new LiteDatabase(DataLocations.GetIndexStateFile());

            InitializeCollections();
            EnsureKeys();
        }

        private void InitializeCollections()
        {
            PartitionEntries = db.GetCollection<PartitionEntry>("partition_entries", BsonAutoId.Int32);
            SubscriptionEntries = db.GetCollection<SubscriptionEntry>("subscription_state_entries", BsonAutoId.Int32);
        }

        private void EnsureKeys()
        {
            // no keys for now! :p
        }

        public ILiteCollection<PartitionEntry>? PartitionEntries { get; set; }
        public ILiteCollection<SubscriptionEntry>? SubscriptionEntries { get; set; }
    }
}
