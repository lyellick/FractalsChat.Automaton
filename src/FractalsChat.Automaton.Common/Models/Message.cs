using FractalsChat.Automaton.Common.Enums;

namespace FractalsChat.Automaton.Common.Models
{
    public class Message
    {
        public ListenerHook Hook { get; set; }
        public string[] Parts { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Body { get; set; }
    }
}
