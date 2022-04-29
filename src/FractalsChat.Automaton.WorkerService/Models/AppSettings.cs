namespace FractalsChat.Automaton.WorkerService.Models
{
    public class AppSettings
    {
        public Timeout Timeout { get; set; }
    }

    public class Timeout
    {
        /// <summary>
        /// The total number of timeouts before ending application.
        /// </summary>
        public int Threshold { get; set; }

        /// <summary>
        /// The total amount of time in seconds to wait to try again.
        /// </summary>
        public int Duration { get; set; }
    }
}
