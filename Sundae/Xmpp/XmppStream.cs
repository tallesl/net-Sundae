namespace Sundae
{
    using Exceptions;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Xml;
    using System;

    internal class XmppStream : IDisposable
    {
        private readonly object _lock = new object();

        private static string[] _valid = new []
        {
            "iq", "message", "presence", "stream:features", "stream:error", "stream:stream",
        };

        private static XmlReaderSettings _settings = new XmlReaderSettings
        {
            IgnoreComments = true,
            IgnoreWhitespace = true,
            IgnoreProcessingInstructions = true,
        };

        private TcpClient _client;

        private Stream _stream;

        private XmlReader _reader;

        private bool _connected = false;

        private bool _disposed = false;

        public void Dispose() => Disconnect();

        internal void Connect(string host, int port, string domain)
        {
            CheckDisposed();

            _client = new TcpClient(host, port);
            _stream = _client.GetStream();
            _connected = true;

            // Opens the XML stream with the server.
            this.SendOpenStream(domain);

            // Not stated in the docs, but blocks until there's data to read.
            _reader = XmlReader.Create(_stream, _settings);

            // Reads the server opening the XML stream.
            ReadOpenStream();
        }

        internal void Disconnect()
        {
            if (_disposed)
                return;

            this.SendCloseStream();

            lock (_lock)
            {
                _reader.Dispose();
                _stream.Dispose();
                _client.Dispose();

                _disposed = true;
            }
        }

        internal void Write(string data)
        {
            CheckConnected();

            var bytes = Encoding.UTF8.GetBytes(data);
            _stream.Write(bytes, 0, bytes.Length);
        }

        internal XmlElement Read()
        {
            // Should be connected.
            CheckConnected();

            lock (_lock)
            {
                // Blocks until something is read.
                MoveReader();

                // The server closed the XML stream on its end.
                if (_reader.NodeType == XmlNodeType.EndElement && _reader.Name == "stream:stream")
                {
                    // Closing on our end.
                    Disconnect();
                    throw new XmlStreamClosedException();
                }

                // Only XML elements are expected.
                if (_reader.NodeType != XmlNodeType.Element)
                    throw new UnexpectedXmlException($"Unexpected \"{_reader.NodeType}\" node:", CurrentElement());

                // Got an unknown tag.
                if (!_valid.Contains(_reader.Name))
                    throw new UnexpectedXmlException($"Unexpected \"{_reader.Name}\" element:", CurrentElement());

                // Reads and returns the whole element.
                return CurrentElement();
            }
        }

        private void ReadOpenStream()
        {
            // Blocks until something is read.
            MoveReader();

            // Ignoring XML declaration if present.
            if (_reader.NodeType == XmlNodeType.XmlDeclaration)
                MoveReader();

            // The XML stream should start by opening a stream tag.
            if (_reader.Name != "stream:stream")
                throw new UnexpectedXmlException("Expected open stream tag, got:", CurrentElement());
        }

        private void MoveReader()
        {
            // Blocks until there's data to read, returns false if less than 4 bytes are written.
            if (!_reader.Read())
                throw new XmlException("No data.");
        }

        // https://stackoverflow.com/a/284406
        private XmlElement CurrentElement()
        {
            using (var inner = _reader.ReadSubtree())
            {
                var doc = new XmlDocument();

                doc.Load(inner);

                return doc.DocumentElement;
            }
        }

        private void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        private void CheckConnected()
        {
            CheckDisposed();

            if (!_connected)
                throw new InvalidOperationException("Not connected.");
        }
    }
}