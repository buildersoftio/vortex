using Cerebro.Core.Models.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cerebro.Core.Models.Common.Clients.Applications;
using Cerebro.Core.Utilities.Json;
using System.Text.Json.Serialization;

namespace Cerebro.Core.Models.Entities.Clients.Applications
{
    public class ApplicationSettings : BaseEntity
    {
        [ForeignKey("Applications")]
        public int ApplicationId { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public bool IsAuthorizationEnabled { get; set; }
        public bool IsConnectionAllowedForAnyAddress { get; set; }

        public List<string> PublicIpRange { get; set; }
        public List<string> PrivateIpRange { get; set; }


        [JsonIgnore]
        [Column("PublicIpRange", TypeName = "json")]
        public string _PublicIpRange
        {
            get
            {
                return PublicIpRange.ToJson();
            }
            set
            {
                PublicIpRange = value.JsonToObject<List<string>>();
            }
        }

        [JsonIgnore]
        [Column("PrivateIpRange", TypeName = "json")]
        public string _PrivateIpRange
        {
            get
            {
                return PrivateIpRange.ToJson();
            }
            set
            {
                PrivateIpRange = value.JsonToObject<List<string>>();
            }
        }

        public ProductionInstanceTypes DefaultProductionInstanceType { get; set; }
        public SubscriptionTypes DefaultSubscriptionType { get; set; }
        public SubscriptionModes DefaultSubscriptionMode { get; set; }
        public ReadInitialPositions DefaultReadInitialPosition { get; set; }
    }
}
