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

            this.OpenStream(domain);

            // Not stated in the docs, but blocks until there's data to read.
            _reader = XmlReader.Create(_stream, _settings);

            ReadOpenStream();
        }

        internal void Disconnect()
        {
            if (_disposed)
                return;

            _reader.Dispose();

            this.CloseStream();

            _stream.Dispose();
            _client.Dispose();

            _disposed = true;
        }

        internal void Write(string data)
        {
            CheckConnected();

            var bytes = Encoding.UTF8.GetBytes(data);
            _stream.Write(bytes, 0, bytes.Length);
        }

        internal XmlElement Read()
        {
            CheckConnected();

            MoveReader();

            if (_reader.NodeType == XmlNodeType.EndElement && _reader.Name == "stream:stream")
            {
                Disconnect();
                throw new XmlStreamClosedException();
            }

            if (_reader.NodeType != XmlNodeType.Element)
                throw new UnexpectedXmlException($"Got an unexpected \"{_reader.NodeType}\" node:", CurrentElement());

            if (!_valid.Contains(_reader.Name))
                throw new UnexpectedXmlException($"Got an unexpected \"{_reader.Name}\" element:", CurrentElement());

            return CurrentElement();
        }

        private void ReadOpenStream()
        {
            MoveReader();

            if (_reader.NodeType == XmlNodeType.XmlDeclaration)
                MoveReader();

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