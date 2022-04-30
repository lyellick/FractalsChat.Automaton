using FractalsChat.Automaton.Common;
using FractalsChat.Automaton.Common.Context;
using FractalsChat.Automaton.Common.Enums;
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

                    // Listener: Network Session Logs
                    networkSession.Listeners.Add(async (message, writer) => {
                        await AddChannelLog(session.ChannelId, message.To, message.From, message.Body);
                    });

                    // Listener: Reminder
                    //      remindme 10 Remind me to do something
                    //      remindchannel 10 Remind the channel to do something
                    networkSession.Listeners.Add(async (message, writer) => {
                        bool isReminder = message.Hook == ListenerHook.REMINDME || message.Hook == ListenerHook.REMINDCHANNEL;
                        if (isReminder)
                        {
                            bool isValidTimespan = int.TryParse(message.Parts[4], out int seconds);

                            if (isReminder && isValidTimespan)
                            {
                                string reminder = string.Join(" ", message.Parts.Skip(5));
                                string to = message.Hook == ListenerHook.REMINDME ? message.From : session.Channel.Name;

                                Task task = new(async () =>
                                {
                                    Thread.Sleep(seconds * 1000);
                                    string body = $"~ Reminder: {reminder}";
                                    await writer.WriteLineAsync($"PRIVMSG {to} :{body}");
                                    await writer.FlushAsync();

                                    await AddChannelLog(session.ChannelId, to, session.Bot.Nickname, body);
                                });

                                task.Start();
                            }
                        }
                    });

                    await networkSession.StartListeningAsync();
                });

                instance.Start();

                instances.Add(instance);
            }

            await Task.WhenAll(instances.ToArray());
        }

        private async Task AddChannelLog(int channelId, string to, string from, string body)
        {
            ChannelLog log = new() { ChannelId = channelId, To = to, From = from, Body = body, Created = DateTimeOffset.UtcNow };

            await _context.ChannelLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}