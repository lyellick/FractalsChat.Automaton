using Figgle;
using FractalsChat.Automaton.Common;
using FractalsChat.Automaton.Common.Context;
using FractalsChat.Automaton.Common.Enums;
using FractalsChat.Automaton.Common.Extensions;
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

                    // Trigger: Change to Active Users in Channel
                    networkSession.OnActiveUsersChange = async (members) => await UpdateActiveChannelUsers(members, session.Channel);

                    // Listener: Network Session Logs
                    networkSession.Listeners.Add(async (message, writer) => {
                        await AddChannelLog(session.ChannelId, message.To, message.From, message.Body);
                    });

                    // Listener: Bot Test
                    networkSession.Listeners.Add((message, writer) =>
                    {
                        if (message.Hook == ListenerHook.BEEP)
                        {
                            Task task = new(async () =>
                            {
                                string body = "boop";
                                await writer.SendAsync(session.Channel.Name, body);
                                await AddChannelLog(session.ChannelId, session.Channel.Name, session.Bot.Nickname, body);
                            });

                            task.Start();
                        }
                    });

                    // Listener: Reminder
                    //      remindme 10 Remind me to do something
                    //      remindchannel 10 Remind the channel to do something
                    networkSession.Listeners.Add((message, writer) =>
                    {
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
                                    await writer.SendAsync(to, body);
                                    await AddChannelLog(session.ChannelId, to, session.Bot.Nickname, body);
                                });

                                task.Start();
                            }
                        }
                    });

                    // Listener: Banner
                    networkSession.Listeners.Add(async (message, writer) => {
                        if (message.Hook == ListenerHook.BANNER)
                        {
                            string bannerMessage = string.Join(" ", message.Parts.Skip(4));
                            string[] banner = FiggleFonts.Small.Render(bannerMessage).Split("\r\n");
                            await writer.SendAsync(session.Channel.Name, banner);
                            foreach (string row in banner)
                                await AddChannelLog(session.ChannelId, session.Channel.Name, session.Bot.Nickname, row);
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

        private async Task UpdateActiveChannelUsers(string[] members, Channel channel)
        {
            channel.UpdateMembers(members);

            await _context.SaveChangesAsync();
        }
    }
}