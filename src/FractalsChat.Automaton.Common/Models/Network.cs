namespace FractalsChat.Automaton.Common.Models
{
    public class Network
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int NetworkId { get; set; }

        /// <summary>
        /// Common name of IRC network.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of IRC network.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Domain name of IRC network.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Port for IRC network.
        /// </summary>
        public int Port { get; set; } = 6667;

        /// <summary>
        /// Collection of sessions the network has.
        /// </summary>
        public virtual ICollection<Session> Sessions { get; set; }
    }
}
