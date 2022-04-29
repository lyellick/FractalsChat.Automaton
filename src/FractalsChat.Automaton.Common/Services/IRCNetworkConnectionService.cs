using FractalsChat.Automaton.Common.Context;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalsChat.Automaton.Common.Services
{
    public class IRCNetworkConnectionService : IIRCNetworkConnectionService
    {
        public readonly ILogger<IRCNetworkConnectionService> _logger;
        public readonly FractalsChatContext _context;

        public IRCNetworkConnectionService(ILogger<IRCNetworkConnectionService> logger, FractalsChatContext context)
        {
            _logger = logger;
            _context = context;
        }
    }

    public interface IIRCNetworkConnectionService
    {
    }
}
