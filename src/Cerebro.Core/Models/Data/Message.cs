namespace Cerebro.Core.Models.Data
{
    public class Message
    {
        public long EntryId { get; set; }

        public byte[] Id { get; set; }
        public byte[] Payload { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        public string NodeOwnerId { get; set; }


        public DateTimeOffset StoredDate { get; set; }
        public DateTimeOffset SentDate { get; set; }
    }
}
