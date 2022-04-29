using FractalsChat.Automaton.Common.Context;
using Microsoft.Extensions.Logging;

namespace FractalsChat.Automaton.Common.Services
{
    public class IRCNetworkListenerService : IIRCNetworkListenerService
    {
        private readonly ILogger<IRCNetworkListenerService> _logger;
        private readonly FractalsChatContext _context;

        public IRCNetworkListenerService(ILogger<IRCNetworkListenerService> logger, FractalsChatContext context)
        {
            _logger = logger;
            _context = context;
        }
    }

    public interface IIRCNetworkListenerService
    {
    }
}
