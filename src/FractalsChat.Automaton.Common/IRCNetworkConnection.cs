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

        public readonly StreamWriter Writer;
        public readonly StreamReader Reader;

        private readonly NetworkStream _stream;
        private readonly TcpClient _client;

        public IRCNetworkConnection(Session session)
        {
            _client = new TcpClient();
            _client.Connect(session.Network.Hostname, session.Network.Port);
            _stream = _client.GetStream();

            Writer = new StreamWriter(_stream);
            Reader = new StreamReader(_stream);
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
