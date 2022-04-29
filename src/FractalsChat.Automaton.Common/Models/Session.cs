using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalsChat.Automaton.Common.Models
{
    public class Session
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int SessionId { get; set; }

        /// <summary>
        /// Foreign key of the channel entity.
        /// </summary>
        public int NetworkId { get; set; }

        /// <summary>
        /// Foreign key of the channel entity.
        /// </summary>
        public int ChannelId { get; set; }

        /// <summary>
        /// Foreign key identifier of the bot enitty.
        /// </summary>
        public int BotId { get; set; }

        /// <summary>
        /// Date and time the session started.
        /// </summary>
        public DateTimeOffset? Started { get; set; } = null;

        /// <summary>
        /// Guid of session.
        /// </summary>
        public Guid SessionGuid { get; set; }

        /// <summary>
        /// Channel information.
        /// </summary>
        public virtual Channel Channel { get; set; }

        /// <summary>
        /// Bot information
        /// </summary>
        public virtual Bot Bot { get; set; }

        /// <summary>
        /// Network information
        /// </summary>
        public virtual Network Network { get; set; }
    }
}
