﻿using FractalsChat.Automaton.Common.Context;
using Microsoft.Extensions.Logging;

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