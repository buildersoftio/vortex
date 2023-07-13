using Cerebro.Core.IO;
using Cerebro.Core.Models.Entities.Addresses;
using Cerebro.Core.Models.Entities.Clients.Applications;
using Microsoft.EntityFrameworkCore;

namespace Cerebro.Infrastructure.DataAccess.ApplicationStateStore
{
    public class ApplicationStateStoreDbContext : DbContext
    {
        private readonly string _applicationStoreFile;

        public ApplicationStateStoreDbContext()
        {
            _applicationStoreFile = ConfigLocations.GetApplicationStateStoreFile();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_applicationStoreFile}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>()
                .HasIndex(p => p.Name).IsUnique();


            modelBuilder.Entity<Application>()
                .HasIndex(p => p.Name).IsUnique();

            modelBuilder.Entity<ApplicationSettings>().Ignore(t => t.PublicIpRange);
            modelBuilder.Entity<ApplicationSettings>().Ignore(t => t.PrivateIpRange);

            modelBuilder.Entity<ApplicationAddressConnection>().Ignore(t => t.ConnectedIPs);
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<AddressSettings> AddressSettings { get; set; }

        // Clients - Applications

        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationSettings> ApplicationSettings { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<ApplicationPermission> ApplicationPermissions { get; set; }
        public DbSet<ApplicationToken> ApplicationTokens { get; set; }
        public DbSet<ApplicationAddressConnection> ApplicationAddressConnections { get; set; }
    }
}
