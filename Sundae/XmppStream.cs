namespace Sundae
{
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

        private readonly string _host;

        private readonly int _port;

        private readonly string _domain;

        private TcpClient _client;

        private Stream _stream;

        private XmlReader _reader;

        internal XmppStream(string host, int port, string domain)
        {
            _host = host;
            _port = port;
            _domain = domain;
        }

        public void Dispose() => Disconnect();

        internal void Connect()
        {
            _client = new TcpClient(_host, _port);
            _stream = _client.GetStream();

            WriteOpenStream();

            // Not stated in the docs, but blocks until there's data to read.
            _reader = XmlReader.Create(_stream, _settings);

            ReadOpenStream();
        }

        internal void Disconnect()
        {
            _reader.Dispose();

            WriteCloseStream();

            _stream.Dispose();
            _client.Dispose();

            _reader = null;
            _stream = null;
            _client = null;
        }

        internal void Write(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);

            _stream.Write(bytes, 0, bytes.Length);
        }

        internal XmlElement Read()
        {
            MoveReader();

            if (_reader.NodeType == XmlNodeType.EndElement && _reader.Name == "stream:stream")
                throw new UnexpectedXmlException("The XML stream was closed:", CurrentElement());

            if (_reader.NodeType != XmlNodeType.Element)
                throw new UnexpectedXmlException($"Got an unexpected \"{_reader.NodeType}\" node:", CurrentElement());

            if (!_valid.Contains(_reader.Name))
                throw new UnexpectedXmlException($"Got an unexpected \"{_reader.Name}\" element:", CurrentElement());

            return CurrentElement();
        }

        private void WriteOpenStream() => Write($"<stream:stream to='{_domain}' xmlns='jabber:client' xmlns:stream='http://etherx.jabber.org/streams' version='1.0'>");

        private void WriteCloseStream() => Write("</stream>");

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
    }
}