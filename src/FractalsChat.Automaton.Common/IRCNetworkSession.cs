using FractalsChat.Automaton.Common.Enums;
using FractalsChat.Automaton.Common.Models;

namespace FractalsChat.Automaton.Common
{
    public class IRCNetworkSession : IDisposable
    {
        public List<Action<Message, StreamWriter>> Listeners;

        private Session _session;
        
        private readonly IRCNetworkConnection _connection;

        public IRCNetworkSession(Session session)
        {
            _session = session;
            _connection = new(_session);
            Listeners = new();
        }

        public async Task StartListeningAsync()
        {
            await ConnectAsync();

            while (_connection.IsConnected)
            {
                try
                {
                    string output = await _connection.Reader.ReadLineAsync();

                    if (!string.IsNullOrEmpty(output))
                    {
                        string[] outputParts = output.Split(' ');

                        if (outputParts[0] == "PING")
                            await KeepAliveAsync();

                        CommandResponse command = GetCommandResponse(outputParts[1]);

                        switch (command)
                        {
                            case CommandResponse.NOMOTD:
                            case CommandResponse.ENDOFMOTD:
                                await JoinChannelAsync();
                                break;
                            case CommandResponse.PRIVMSG:
                                if (outputParts.Length > 2)
                                {
                                    Message message = new() 
                                    { 
                                        Parts = outputParts,
                                        Reciver = outputParts[2],
                                        Sender = output.Split('!')[0][1..],
                                        Body = output.Split(':')[2]
                                    };

                                    foreach (var listener in Listeners)
                                        listener(message, _connection.Writer);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Dispose();
                }
            }
        }

        private async Task ConnectAsync()
        {
            await _connection.Writer.WriteLineAsync($"USER {_session.Bot.Ident} * 8 {_session.Bot.Gecos}");

            await _connection.Writer.WriteLineAsync($"NICK {_session.Bot.Nickname}");

            await _connection.Writer.WriteLineAsync($"PRIVMSG NickServ :IDENTIFY {_session.Bot.Nickname} {_session.Bot.Password}");

            await _connection.Writer.FlushAsync();
        }

        private async Task JoinChannelAsync(string message = null)
        {
            await _connection.Writer.WriteLineAsync($"JOIN {_session.Channel.Name}");

            if (!string.IsNullOrEmpty(message))
                await _connection.Writer.WriteLineAsync($"PRIVMSG {_session.Channel.Name} :{message}");

            await _connection.Writer.FlushAsync();
        }

        private async Task KeepAliveAsync()
        {
            await _connection.Writer.WriteLineAsync("PONG");

            await _connection.Writer.FlushAsync();
        }

        private static CommandResponse GetCommandResponse(string command)
        {
            CommandResponse commandResponse;

            bool isCommandResponseCode = int.TryParse(command, out int commandResponseCode);

            if (isCommandResponseCode)
            {
                commandResponse = (CommandResponse)commandResponseCode;
            }
            else
            {
                Enum.TryParse(command, out commandResponse);
            }

            return commandResponse;
        }

        #region IDisposable
        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                    _connection.Dispose();

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
