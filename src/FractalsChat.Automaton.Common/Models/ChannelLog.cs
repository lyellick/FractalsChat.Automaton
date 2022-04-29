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
        /// Nickname of the sender.
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// Message the sender sent.
        /// </summary>
        public string Message { get; set; }

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
