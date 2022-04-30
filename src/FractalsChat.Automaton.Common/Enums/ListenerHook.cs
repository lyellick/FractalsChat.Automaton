namespace FractalsChat.Automaton.Common.Enums
{
    public enum ListenerHook
    {
        /// <summary>
        /// Unassigned hook.
        /// </summary>
        UNASSIGNED,

        /// <summary>
        /// Reminds the sended of a message in x seconds.
        /// </summary>
        REMINDME,

        /// <summary>
        /// Reminds the channel of a message in x seconds.
        /// </summary>
        REMINDCHANNEL
    }
}
