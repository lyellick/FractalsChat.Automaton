using FractalsChat.Automaton.Common;
using FractalsChat.Automaton.Common.Context;
using FractalsChat.Automaton.Common.Models;
using FractalsChat.Automaton.WorkerService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FractalsChat.Automaton.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private readonly AppSettings _settings;

        private readonly FractalsChatContext _context;

        public Worker(IOptions<AppSettings> options, ILogger<Worker> logger, IConfiguration configuration)
        {
            _context = new(new DbContextOptionsBuilder<FractalsChatContext>().UseLazyLoadingProxies().UseSqlite(configuration.GetConnectionString("DefaultConnection")).Options);
            _settings = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<Task> instances = new();

            await _context.Database.EnsureCreatedAsync();

            List<Session> sessions = await _context.Sessions.ToListAsync();

            foreach (Session session in sessions)
            {
                Task instance = new(async () => {
                    using IRCNetworkSession networkSession = new(session);

                    networkSession.Listeners.Add(async (message, messageParts, command, writer) => {
                        await writer.WriteLineAsync($"PRIVMSG {session.Channel.Name} :test");
                        await writer.FlushAsync();
                    });

                    await networkSession.StartListeningAsync();
                });

                instance.Start();

                instances.Add(instance);
            }

            await Task.WhenAll(instances.ToArray());
        }
    }
}