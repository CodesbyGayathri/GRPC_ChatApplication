using System.Collections.Generic;

namespace ChatMVC.Models
{
    public class ChatViewModel
    {
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
        public string Username { get; set; } // Add this property
        public List<string> ActiveUsers { get; set; }
    }
}
