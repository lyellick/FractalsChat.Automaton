namespace FractalsChat.Automaton.Common.Models
{
    public class Bot
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int BotId { get; set; }

        /// <summary>
        /// Nickname of bot.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Description of bot.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Real name of bot.
        /// </summary>
        public string Gecos { get; set; }

        /// <summary>
        /// ID of bot. Usually the nickname.
        /// </summary>
        public string Ident { get; set; }

        /// <summary>
        /// Passowrd of bot for a specific network.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Date and time the bot was created.
        /// </summary>
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// Date and time the bot was updated.
        /// </summary>
        public DateTimeOffset? Modified { get; set; } = null;

        /// <summary>
        /// Unique guid of the bot entity.
        /// </summary>
        public Guid botGuid { get; set; }

    }
}
