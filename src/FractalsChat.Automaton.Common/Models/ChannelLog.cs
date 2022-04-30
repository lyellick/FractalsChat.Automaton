namespace FractalsChat.Automaton.Common.Models
{
    public class ChannelLog
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int ChannelLogId { get; set; }

        /// <summary>
        /// Foreign key of the channel entity.
        /// </summary>
        public int ChannelId { get; set; }

        /// <summary>
        /// Name of reciver.
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Nickname of the sender.
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Message the sender sent.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Date and time the log was created.
        /// </summary>
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// Channel information.
        /// </summary>
        public virtual Channel Channel { get; set; }
    }
}
