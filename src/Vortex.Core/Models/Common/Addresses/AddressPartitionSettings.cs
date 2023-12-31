﻿using System.ComponentModel.DataAnnotations;

namespace Vortex.Core.Models.Common.Addresses
{
    public class AddressPartitionSettings
    {
        [Range(1, 20)]
        public int PartitionNumber { get; set; }

        [Range(1, 4)]
        public int PartitionThreadLimit { get; set; }

        public Dictionary<int, Partition> Partitions { get; set; }

        public AddressPartitionSettings()
        {
            Partitions = new Dictionary<int, Partition>();
        }
    }
}
