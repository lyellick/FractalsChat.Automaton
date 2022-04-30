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
        /// Reminds the sended of a message in x seconds.
        /// </summary>
        REMINDME,

        /// <summary>
        /// Reminds the channel of a message in x seconds.
        /// </summary>
        REMINDCHANNEL,

        /// <summary>
        /// Writes a large banner to the chat.
        /// </summary>
        BANNER,

        /// <summary>
        /// Writes the weather for a location.
        /// </summary>
        WEATHER
    }
}
