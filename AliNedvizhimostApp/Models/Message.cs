using System;

namespace AliNedvizhimostApp.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int PropertyId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
