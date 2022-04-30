using FractalsChat.Automaton.Common.Enums;
using FractalsChat.Automaton.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FractalsChat.Automaton.Common
{
    public class IRCNetworkConnection : IDisposable
    {
        public bool IsConnected => _client.Connected;
        public List<Action<string, string[], CommandResponse, IRCNetworkConnection>> Listeners;

        public readonly StreamWriter Writer;
        public readonly StreamReader Reader;

        private Session _session;

        private readonly NetworkStream _stream;
        private readonly TcpClient _client;

        public IRCNetworkConnection(Session session)
        {
            _client = new TcpClient();
            _client.Connect(session.Network.Hostname, session.Network.Port);
            _stream = _client.GetStream();
            _session = session;

            Writer = new StreamWriter(_stream);
            Reader = new StreamReader(_stream);
            Listeners = new();
        }

        public async Task ConnectAsync()
        {
            await Writer.WriteLineAsync($"USER {_session.Bot.Ident} * 8 {_session.Bot.Gecos}");

            await Writer.WriteLineAsync($"NICK {_session.Bot.Nickname}");

            await Writer.WriteLineAsync($"PRIVMSG NickServ :IDENTIFY {_session.Bot.Nickname} {_session.Bot.Password}");

            await Writer.FlushAsync();
        }

        public async Task JoinChannelAsync(string message = null)
        {
            await Writer.WriteLineAsync($"JOIN {_session.Channel.Name}");

            if (!string.IsNullOrEmpty(message))
                await Writer.WriteLineAsync($"PRIVMSG {_session.Channel.Name} :{message}");

            await Writer.FlushAsync();
        }

        public async Task KeepAliveAsync()
        {
            await Writer.WriteLineAsync("PONG");

            await Writer.FlushAsync();
        }

        public static CommandResponse GetCommandResponse(string command)
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
        private bool _disposed;

        private void CloseInstances()
        {
            Writer.Close();
            Reader.Close();
            _stream.Close();
            _client.Close();

            _disposed = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    CloseInstances();
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
