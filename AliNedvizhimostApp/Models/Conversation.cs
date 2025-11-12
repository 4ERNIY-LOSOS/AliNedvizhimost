using System;

namespace AliNedvizhimostApp.Models
{
    public class Conversation
    {
        public int PropertyId { get; set; }
        public string PropertyTitle { get; set; }
        public string PropertyAddress { get; set; }
        public int OtherUserId { get; set; }
        public string OtherUserFirstName { get; set; }
        public string OtherUserLastName { get; set; }
        public DateTime LastMessageTimestamp { get; set; }
    }
}
