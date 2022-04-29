using FractalsChat.Automaton.Common.Context;
using Microsoft.Extensions.Logging;

namespace FractalsChat.Automaton.Common.Services
{
    public class IRCNetworkAutomationService : IIRCNetworkAutomationService
    {
        public readonly ILogger<IRCNetworkAutomationService> _logger;
        public readonly FractalsChatContext _context;

        public IRCNetworkAutomationService(ILogger<IRCNetworkAutomationService> logger, FractalsChatContext context)
        {
            _logger = logger;
            _context = context;
        }
    }

    public interface IIRCNetworkAutomationService
    {
    }
}
