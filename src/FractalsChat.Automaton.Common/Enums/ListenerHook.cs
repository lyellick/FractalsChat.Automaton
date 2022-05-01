namespace FractalsChat.Automaton.Common.Enums
{
    public enum ListenerHook
    {
        /// <summary>
        /// Unassigned hook.
        /// </summary>
        UNASSIGNED,

        /// <summary>
        /// Used to test user is listening to chat.
        /// </summary>
        BEEP,

        /// <summary>
        /// Reminds the sended or channel of a message in x seconds.
        /// </summary>
        REMIND,

        /// <summary>
        /// Writes a large banner to the chat.
        /// </summary>
        BANNER,

        /// <summary>
        /// Writes the weather for a location to sender or channel.
        /// </summary>
        WEATHER,

        /// <summary>
        /// Writes a peice of widom to sender or channel.
        /// </summary>
        WISDOM,

        /// <summary>
        /// Writes an activity for the sender or channel.
        /// </summary>
        IMBORED
    }
}
