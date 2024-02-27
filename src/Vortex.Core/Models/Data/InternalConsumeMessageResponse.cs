namespace Vortex.Core.Models.Data
{
    public class InternalConsumeMessageResponse
    {
        public long EntryId { get; set; }
        public int PartitionId { get; set; }

        public byte[] MessageId { get; set; }
        public byte[] MessagePayload { get; set; }

       public Dictionary<string, string> MessageHeader { get; set; }
        public string SourceApplication { get; set; }

        public DateTime SentDate { get; set; }
    }
}
