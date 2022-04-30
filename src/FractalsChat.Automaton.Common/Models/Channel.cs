namespace FractalsChat.Automaton.Common.Models
{
    public partial class Channel
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int ChannelId { get; set; }

        /// <summary>
        /// Common name of channel in IRC network.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of channel in IRC network.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Comma seperated list of members active in the channel.
        /// </summary>
        public string ActiveMembers { get; set; } = null;

        /// <summary>
        /// Comma seperated list of members in the channel over time.
        /// </summary>
        public string HistoricalMembers { get; set; } = null;

        /// <summary>
        /// Date and time the channel was created.
        /// </summary>
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// Date and time the channel was updated.
        /// </summary>
        public DateTimeOffset? Modified { get; set; } = null;

        /// <summary>
        /// Channel log information.
        /// </summary>
        public virtual ICollection<ChannelLog> ChannelLogs { get; set; }
    }

    public partial class Channel
    {
        public string[] GetActiveMembers() => ActiveMembers != null ? ActiveMembers.Split('\u002C') : Array.Empty<string>();

        public string[] GetHistoricalMembers() => HistoricalMembers != null ? HistoricalMembers.Split('\u002C') : Array.Empty<string>();

        public void UpdateMembers(string[] members)
        {
            ActiveMembers = string.Join('\u002C', members);

            List<string> activeMembers = GetActiveMembers().ToList();
            List<string> historicalMembers = GetHistoricalMembers().ToList();

            historicalMembers.AddRange(activeMembers.Except(historicalMembers));

            HistoricalMembers = string.Join('\u002C', historicalMembers);

            Modified = DateTimeOffset.UtcNow;
        }
    }
}
