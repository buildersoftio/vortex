﻿using MessagePack;

namespace Vortex.Core.Models.Data
{

    [MessagePackObject]
    public class Message
    {
        [Key(0)]
        public long EntryId { get; set; }

        [Key(1)]
        public byte[]? MessageId { get; set; }
        [Key(2)]
        public byte[] MessagePayload { get; set; }

        [Key(3)]
        public Dictionary<string, string> MessageHeaders { get; set; }

        // from the client
        [Key(4)]
        public string SourceApplication { get; set; }

        // from the serverContext
        [Key(5)]
        public string HostApplication { get; set; }


        [Key(6)]
        public DateTimeOffset StoredDate { get; set; }
        [Key(7)]
        public DateTimeOffset SentDate { get; set; }
    }
}
