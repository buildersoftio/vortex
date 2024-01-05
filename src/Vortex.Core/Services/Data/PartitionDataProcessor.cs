using MessagePack;
using Vortex.Core.Abstractions.Background;
using Vortex.Core.Abstractions.Clustering;
using Vortex.Core.Abstractions.Services;
using Vortex.Core.Abstractions.Services.Data;
using Vortex.Core.Models.Configurations;
using Vortex.Core.Models.Data;
using Vortex.Core.Models.Entities.Addresses;
using Vortex.Core.Models.Entities.Entries;

namespace Vortex.Core.Services.Data
{
    public class PartitionDataProcessor : ParallelBackgroundQueueServiceBase<PartitionMessage>
    {
        private readonly IPartitionEntryService _partitionEntryService;
        private readonly Address _address;
        private readonly PartitionEntry _partitionEntry;
        private readonly IPartitionDataService<byte> _partitionDataService;
        private readonly NodeConfiguration _nodeConfiguration;

        private readonly IClusterStateRepository _clusterStateRepository;

        private static TimeSpan GetPartitionEntityFlushPeriod(NodeConfiguration nodeConfiguration)
        {
            return new TimeSpan(0, 0, nodeConfiguration.BackgroundPositionEntry_FlushInterval);
        }

        public PartitionDataProcessor(IPartitionEntryService partitionEntryService,
            Address address,
            PartitionEntry partitionEntry,
            IPartitionDataService<byte> partitionDataService,
            NodeConfiguration nodeConfiguration,
            IClusterStateRepository clusterStateRepository) : base(address.Settings.PartitionSettings.PartitionThreadLimit, period: GetPartitionEntityFlushPeriod(nodeConfiguration))
        {
            _partitionEntryService = partitionEntryService;
            _address = address;
            _partitionEntry = partitionEntry;
            _partitionDataService = partitionDataService;
            _nodeConfiguration = nodeConfiguration;

            // adding support for cluster communication
            _clusterStateRepository = clusterStateRepository;

            base.StartTimer();
        }

        public override void Handle(PartitionMessage request)
        {
            // check if this message should go to other nodes in the cluster
            if (_partitionEntry.NodeOwner != _nodeConfiguration.NodeId)
            {
                var requestAsSpan = new ReadOnlySpan<PartitionMessage>(ref request);
                TransmitMessageToNode(_partitionEntry.NodeOwner, requestAsSpan);

                return;
            }

            //prepare the message to store..
            _partitionEntry.CurrentEntry = _partitionEntry.CurrentEntry + 1;
            long entryId = _partitionEntry.CurrentEntry;

            // do breaking of indexes for each partition; this will enable new based on dateTime consumption.
            CoordinatePositionIndex(entryId, _partitionEntry);

            var messageToStore = new Message()
            {
                EntryId = entryId,
                MessageHeaders = request.MessageHeaders,
                MessageId = request.MessageId,
                MessagePayload = request.MessagePayload,
                HostApplication = request.HostApplication,
                SourceApplication = request.SourceApplication,
                SentDate = request.SentDate,
                StoredDate = request.StoredDate
            };

            byte[] entry = MessagePackSerializer.Serialize(entryId);
            byte[] message = MessagePackSerializer.Serialize(messageToStore);

            _partitionDataService.Put(entry, message);
        }


        int cleanMemoryCount = 0;
        public override void OnTimer_Callback(object? state)
        {
            // TODO: In case data is the same store the same state.

            // updating the Entry position for the partition.
            _partitionEntry.UpdatedAt = DateTime.UtcNow;
            _partitionEntry.UpdatedBy = "BACKGROUND_data_processor";
            _partitionEntryService.UpdatePartitionEntry(_partitionEntry);


            cleanMemoryCount++;
            if(cleanMemoryCount == 6)
            {
                GC.Collect(2);
                GC.WaitForPendingFinalizers();

                cleanMemoryCount = 0;
            }
        }

        private void CoordinatePositionIndex(long entryId, PartitionEntry partitionEntry)
        {
            var currentDate = DateTime.Now;
            string positionIndex = "";
            switch (partitionEntry.MessageIndexType)
            {
                case Models.Common.Addresses.MessageIndexTypes.HOURLY:
                    positionIndex = currentDate.ToString("yyyy-MM-dd HH");
                    break;
                case Models.Common.Addresses.MessageIndexTypes.DAILY:
                    positionIndex = currentDate.ToString("yyyy-MM-dd");
                    break;
                case Models.Common.Addresses.MessageIndexTypes.MONTHLY:
                    positionIndex = currentDate.ToString("yyyy-MM");
                    break;
                default:
                    positionIndex = currentDate.ToString("yyyy-MM-dd");
                    break;
            }

            if (partitionEntry.Positions.ContainsKey(positionIndex) != true)
            {
                // adding PositionEntry point
                partitionEntry.Positions.Add(positionIndex, new IndexPosition() { StartEntryPosition = entryId });
                var positionWithoutEndEntryPoint = partitionEntry.Positions.Where(x => x.Value.EndEntryPosition == null).FirstOrDefault().Key;
                partitionEntry.Positions[positionWithoutEndEntryPoint].EndEntryPosition = entryId - 1;
            }
        }

        private void TransmitMessageToNode(string nodeOwner, ReadOnlySpan<PartitionMessage> request)
        {
            // call the cluster distribution via gRPC, in case if fails, store it in partition temporary.
            var result = _clusterStateRepository
                            .GetNodeClient(nodeOwner)!
                            .RequestDataDistribution(_address.Alias, request[0]).Result;

            if (result != true)
            {
                // store the message temporary in the local side.
                // prepare the message to store..
                _partitionEntry.ClusterTemporaryCurrentEntry = _partitionEntry.ClusterTemporaryCurrentEntry + 1;
                long tempEntryId = _partitionEntry.ClusterTemporaryCurrentEntry;


                byte[] entry = MessagePackSerializer.Serialize(tempEntryId);
                byte[] message = MessagePackSerializer.Serialize(request[0]);

                _partitionDataService.PutTemporaryForDistribution(entry, message);
            }
        }
    }
}
