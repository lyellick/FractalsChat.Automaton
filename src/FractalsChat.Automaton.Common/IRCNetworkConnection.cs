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
        /// <summary>
        /// Returns the connection status for this instance.
        /// </summary>
        public bool IsConnected => _client.Connected;

        /// <summary>
        /// The time when the connection was opened.
        /// </summary>
        public readonly DateTimeOffset Opened;

        /// <summary>
        /// Write buffer for the connection.
        /// </summary>
        public readonly StreamWriter Writer;

        /// <summary>
        /// Read bugger for the connection.
        /// </summary>
        public readonly StreamReader Reader;

        /// <summary>
        /// Unique identifier of the connection instance.
        /// </summary>
        public readonly Guid Identifier;

        /// <summary>
        /// Hostname used to open the connection.
        /// </summary>
        public readonly string Hostname;

        /// <summary>
        /// Post used to open the connection.
        /// </summary>
        public readonly int Port;

        private readonly NetworkStream _stream;
        private readonly TcpClient _client;

        public IRCNetworkConnection(string hostname, int port)
        {
            _client = new TcpClient(hostname, port);
            _stream = _client.GetStream();

            Writer = new StreamWriter(_stream);
            Reader = new StreamReader(_stream);

            Opened = DateTimeOffset.UtcNow;
            Identifier = Guid.NewGuid();
            Hostname = hostname;
            Port = port;
        }

        private bool _disposed;

        /// <summary>
        /// Closes all instances where a connection has been opened.
        /// </summary>
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

        /// <summary>
        /// Closes all underlying connections used.
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
