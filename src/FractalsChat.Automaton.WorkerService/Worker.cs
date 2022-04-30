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

        public readonly IServiceProvider _services;

        public Worker(IOptions<AppSettings> options, ILogger<Worker> logger, IServiceProvider services)
        {
            _services = services;
            _settings = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<Task> instances = new();

            DbContextOptionsBuilder<FractalsChatContext> builder = new DbContextOptionsBuilder<FractalsChatContext>();

            builder.UseLazyLoadingProxies().UseSqlite("Data Source=Data/fractalschat.db");

            FractalsChatContext context = new FractalsChatContext(builder.Options);

            await context.Database.EnsureCreatedAsync();

            List<Session> sessions = await context.Sessions.ToListAsync();

            foreach (Session session in sessions)
            {
                Task instance = new(async () => {
                    using IRCNetworkConnection connection = new(session);
                    
                    await connection.ConnectAsync();

                    IRCNetworkListener listener = new(connection);

                    listener.Listeners.Add(async (message, messageParts, command, connection) => {
                        await connection.Writer.WriteLineAsync($"PRIVMSG {session.Channel.Name} :test");
                        await connection.Writer.FlushAsync();
                    });

                    await listener.StartListeningAsync();
                });

                instance.Start();

                instances.Add(instance);
            }

            await Task.WhenAll(instances.ToArray());
        }
    }
}