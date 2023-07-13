using Cerebro.Core.Models.Common.Clients.Applications;
using Cerebro.Core.Models.Entities.Base;
using Cerebro.Core.Utilities.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cerebro.Core.Models.Entities.Clients.Applications
{
    public class ApplicationAddressConnection : BaseEntity
    {
        public Guid Id { get; set; }

        [ForeignKey("Applications")]
        public int ApplicationId { get; set; }

        [ForeignKey("Addresses")]
        public int AddressId { get; set; }

        public ApplicationConnectionTypes ApplicationConnectionType { get; set; }


        public DateTimeOffset FirstConnectionDate { get; set; }
        public DateTimeOffset? LastConnectionDate { get; set; }


        public ProductionInstanceTypes ProductionInstanceType { get; set; }
        public SubscriptionTypes SubscriptionType { get; set; }
        public SubscriptionModes SubscriptionMode { get; set; }
        public ReadInitialPositions ReadInitialPosition { get; set; }


        public List<string>? ConnectedIPs { get; set; }


        [Column("ConnectedIPs", TypeName = "json")]
        public string _ConnectedIPs
        {
            get
            {
                return ConnectedIPs.ToJson();
            }
            set
            {
                ConnectedIPs = value.JsonToObject<List<string>>();
            }
        }
    }
}
