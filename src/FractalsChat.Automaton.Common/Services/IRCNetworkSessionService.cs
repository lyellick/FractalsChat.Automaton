using FractalsChat.Automaton.Common.Context;
using FractalsChat.Automaton.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FractalsChat.Automaton.Common.Services
{
    public class IRCNetworkSessionService : IIRCNetworkSessionService
    {
        private readonly ILogger<IRCNetworkSessionService> _logger;
        private readonly FractalsChatContext _context;

        private Session[] _sessions { get; set; }

        public IRCNetworkSessionService(ILogger<IRCNetworkSessionService> logger, FractalsChatContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task LoadSessionsAsync()
        {
            _sessions = await _context.Sessions.ToArrayAsync();
        }
    }

    public interface IIRCNetworkSessionService
    {
        Task LoadSessionsAsync();
    }
}
