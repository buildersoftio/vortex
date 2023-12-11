namespace Vortex.Core.Models.Data
{
    public class PartitionMessage
    {
        public byte[]? MessageId { get; set; }
        public byte[] MessagePayload { get; set; }
        public Dictionary<string, string> MessageHeaders { get; set; }

        // if the user is sending the message to specific partition
        public int? PartitionIndex { get; set; }


        // from the client
        public string SourceApplication { get; set; }

        // from the serverContext
        public string HostApplication { get; set; }

        public DateTimeOffset StoredDate { get; set; }
        public DateTimeOffset SentDate { get; set; }
    }
}
