using FractalsChat.Automaton.Common.Enums;

namespace FractalsChat.Automaton.Common
{
    public class IRCNetworkListener
    {
        public List<Action<string, string[], CommandResponse, IRCNetworkConnection>> Listeners;

        private readonly IRCNetworkConnection _connection;

        public IRCNetworkListener(IRCNetworkConnection connection)
        {
            _connection = connection;
            Listeners = new();
        }

        public async Task StartListeningAsync()
        {
            while (_connection.IsConnected)
            {
                try
                {
                    string message = await _connection.Reader.ReadLineAsync();

                    if (!string.IsNullOrEmpty(message))
                    {
                        string[] messageParts = message.Split(' ');

                        if (messageParts[0] == "PING")
                            await _connection.KeepAliveAsync();

                        CommandResponse command = IRCNetworkConnection.GetCommandResponse(messageParts[1]);

                        switch (command)
                        {
                            case CommandResponse.NOMOTD:
                            case CommandResponse.ENDOFMOTD:
                                await _connection.JoinChannelAsync();
                                break;
                            case CommandResponse.PRIVMSG:
                                if (messageParts.Length > 2)
                                    foreach (var listener in Listeners)
                                        listener(message, messageParts, command, _connection);
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _connection.Dispose();
                }
            }
        }
    }
}
