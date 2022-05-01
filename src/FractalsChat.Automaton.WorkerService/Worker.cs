using Figgle;
using FractalsChat.Automaton.Common;
using FractalsChat.Automaton.Common.Context;
using FractalsChat.Automaton.Common.Enums;
using FractalsChat.Automaton.Common.Extensions;
using FractalsChat.Automaton.Common.Models;
using FractalsChat.Automaton.WorkerService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

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
                    using IRCNetworkSession networkSession = new(session, _settings.KeepAlive);

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

                    // Listener: Weather
                    //      wetherme area,region
                    //      wetherchannel area,region
                    networkSession.Listeners.Add(async (message, writer) => {
                        bool isWeather = message.Hook == ListenerHook.WEATHERME || message.Hook == ListenerHook.WEATHERCHANNEL;
                        if (isWeather)
                        {
                            string location = string.Join(" ", message.Parts.Skip(4));
                            using HttpClient client = new();
                            string json = await client.GetStringAsync($@"https://wttr.in/{location}?format=j1");
                            try
                            {
                                JObject weather = JsonConvert.DeserializeObject<JObject>(json);
                                
                                string area = weather["nearest_area"][0]["areaName"][0]["value"].Value<string>();
                                string region = weather["nearest_area"][0]["region"][0]["value"].Value<string>();
                                string latitude = weather["nearest_area"][0]["latitude"].Value<string>();
                                string longitude = weather["nearest_area"][0]["longitude"].Value<string>();
                                string highTemp = weather["weather"][0]["maxtempF"].Value<string>();
                                string lowTemp = weather["weather"][0]["mintempF"].Value<string>();
                                string currentTemp = weather["current_condition"][0]["temp_F"]  .Value<string>();
                                string description = weather["current_condition"][0]["weatherDesc"][0]["value"].Value<string>();
                                string sunrise = weather["weather"][0]["astronomy"][0]["sunrise"].Value<string>();
                                string sunset = weather["weather"][0]["astronomy"][0]["sunset"].Value<string>();
                                string moon = weather["weather"][0]["astronomy"][0]["moon_phase"].Value<string>();

                                string[] body = new string[] {
                                    $"~ Weather Report for {area}, {region} ({latitude}, {longitude}):",
                                    $"~     {description}",
                                    $"~     F\u00B0 {currentTemp} (\u25B2 {highTemp}\u00B0 \u25BC {lowTemp}\u00B0)",
                                    $"~     Sunrise {sunrise} -> Sunset {sunset}",
                                    $"~     Moon: {moon}"
                                };

                                string to = message.Hook == ListenerHook.WEATHERME ? message.From : session.Channel.Name;

                                await writer.SendAsync(to, body);
                                await AddChannelLog(session.ChannelId, to, session.Bot.Nickname, body);
                            }
                            catch
                            {

                            }
                        }
                    });

                    // Listener: Wisdom
                    //      wisdomme
                    //      widsomchannel
                    networkSession.Listeners.Add(async (message, writer) => {
                        bool isWisdom = message.Hook == ListenerHook.WISDOMME || message.Hook == ListenerHook.WISDOMCHANNEL;
                        if (isWisdom)
                        {
                            using HttpClient client = new();
                            string json = await client.GetStringAsync($@"https://api.kanye.rest/");
                            try
                            {
                                JObject thinking = JsonConvert.DeserializeObject<JObject>(json);

                                string wisdom = thinking["quote"].Value<string>();

                                string body = $"~ Wisdom: {wisdom}";

                                string to = message.Hook == ListenerHook.WISDOMME ? message.From : session.Channel.Name;

                                await writer.SendAsync(to, body);
                                await AddChannelLog(session.ChannelId, to, session.Bot.Nickname, body);
                            }
                            catch
                            {

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

        private async Task AddChannelLog(int channelId, string to, string from, string[] body)
        {
            List<ChannelLog> logs = new List<ChannelLog>();

            foreach (string row in body)
                logs.Add(new() { ChannelId = channelId, To = to, From = from, Body = row, Created = DateTimeOffset.UtcNow });

            await _context.ChannelLogs.AddRangeAsync(logs);
            await _context.SaveChangesAsync();
        }

        private async Task UpdateActiveChannelUsers(string[] members, Channel channel)
        {
            channel.UpdateMembers(members);

            await _context.SaveChangesAsync();
        }
    }
}