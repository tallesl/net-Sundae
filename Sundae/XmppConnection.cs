namespace Sundae
{
    using Exceptions;
    using System;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using Sundae.Stanzas;
    using static Sundae.Stanzas.ErrorStanza;

    public class XmppConnection : IDisposable
    {
        private readonly XmppStream _stream;

        private CancellationTokenSource _tokenSource;

        public XmppConnection(string host, int port) : this(host, port, host) { }

        public XmppConnection(string host, int port, string domain) => _stream = new XmppStream(host, port, domain);

        public event EventHandler<XmlElement> OnElement;

        public event EventHandler<Exception> OnException;

        public event EventHandler<ErrorStanza> OnError;

        public void Connect()
        {
            _stream.Connect();
            _tokenSource = new CancellationTokenSource();

            RunTask(Read, _tokenSource.Token);
        }

        public void Disconnect() => _Disconnect(true);

        public void Dispose() => _Disconnect(true);

        public void SendCustom(string data) => _stream.Write(data);

        public void SendCustom(XmlElement element) => SendCustom(element.OuterXml);

        private void Read()
        {
            XmlElement element;

            try
            {
                // Getting next element on stream (blocking call).
                element = _stream.Read();
            }
            catch (XmlStreamClosedException)
            {
                // Disconnecting if got a </stream:stream>.
                _Disconnect(false);
                return;
            }

            // Raising the XmlElement.
            OnElement?.Invoke(this, element);

            ErrorStanza error;

            // Trying to parse the stanza as an error.
            if (TryGetError(element, out error))
            {
                // Raising the error.
                OnError?.Invoke(this, error);
                return;
            }
        }

        private void RunTask(Action action, CancellationToken token)
        {
            Task.Run(() =>
            {
                try
                {
                    while (!token.IsCancellationRequested)
                        action();
                }
                catch (Exception e)
                {
                    OnException?.Invoke(this, e);
                }
            });
        }

        private void _Disconnect(bool disconnectStream)
        {
            _tokenSource.Cancel();
            _tokenSource.Dispose();

            if (disconnectStream)
                _stream.Disconnect();
        }
    }
}