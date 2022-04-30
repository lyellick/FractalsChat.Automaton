namespace FractalsChat.Automaton.WorkerService.Models
{
    public class AppSettings
    {
        /// <summary>
        /// Amount of time in seconds to send ping to network to keep connection opened.
        /// </summary>
        public int KeepAlive { get; set; }
    }
}
