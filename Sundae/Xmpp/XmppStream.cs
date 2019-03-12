namespace Sundae
{
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Xml;
    using System;
    using static XmlEncode;

    internal class XmppStream : IDisposable
    {
        private readonly object _readLock = new object();

        private readonly object _writeLock = new object();

        private static XmlReaderSettings _settings = new XmlReaderSettings
        {
            IgnoreComments = true,
            IgnoreWhitespace = true,
            IgnoreProcessingInstructions = true,
        };

        private TcpClient _client;

        private Stream _stream;

        private XmlReader _reader;

        private XmlElement _lastElement;

        private Exception _innerDisposed;

        private volatile bool _connected = false;

        private volatile bool _disposed = false;

        public void Dispose()
        {
            if (_disposed)
                return;

            WriteCloseStream();

            lock (_writeLock)
            lock (_readLock)
            {
                _disposed = true;
                _connected = false;

                _reader.Dispose();
                _stream.Dispose();
                _client.Dispose();
            }
        }

        public void Dispose(Exception innerException)
        {
            _innerDisposed = innerException;
            Dispose();
        }

        internal void Connect(string host, int port, string domain)
        {
            lock (_readLock)
            lock (_writeLock)
            {
                ShouldNotBeConnected();

                _client = new TcpClient(host, port);
                _stream = _client.GetStream();
                _connected = true;

                // Opens the XML stream with the server.
                WriteOpenStream(domain);

                // Not stated in the docs, but blocks until there's data to read.
                _reader = XmlReader.Create(_stream, _settings);

                // Reads the server opening the XML stream.
                ReadOpenStream();
            }
        }

        internal void Write(string xml)
        {
            lock (_writeLock)
            {
                ShouldBeConnected();
                _Write(xml);
            }
        }

        internal XmlElement Read()
        {
            lock (_readLock)
            {
                // Should be connected.
                ShouldBeConnected();

                // Blocks until something is read.
                MoveReader();

                // The server closed the XML stream on its end.
                if (_reader.NodeType == XmlNodeType.EndElement && _reader.Name == "stream:stream")
                    throw new XmlStreamClosedException(_lastElement);

                // Only XML elements are expected.
                if (_reader.NodeType != XmlNodeType.Element)
                    throw new UnexpectedXmlException($"Unexpected \"{_reader.NodeType}\" node:", CurrentElement());

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

        private void WriteOpenStream(string domain)
        {
            Encode(ref domain);

            _Write($"<stream:stream to='{domain}' xmlns='jabber:client' " +
                "xmlns:stream='http://etherx.jabber.org/streams' version='1.0'>");
        }

        private void WriteCloseStream() => _Write("</stream:stream>");

        private void _Write(string xml)
        {
            var bytes = Encoding.UTF8.GetBytes(xml);
            _stream.Write(bytes, 0, bytes.Length);
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

                _lastElement = doc.DocumentElement;

                return doc.DocumentElement;
            }
        }

        private void ShouldBeConnected()
        {
            ShouldNotBeDisposed();

            if (!_connected)
                throw new InvalidOperationException("Not connected.");
        }

        private void ShouldNotBeConnected()
        {
            ShouldNotBeDisposed();

            if (_connected)
                throw new InvalidOperationException("Already connected.");
        }

        private void ShouldNotBeDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException("The XMPP connection was disposed.", _innerDisposed);
        }
    }
}