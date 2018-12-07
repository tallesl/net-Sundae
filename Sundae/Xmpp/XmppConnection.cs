namespace Sundae
{
    using Exceptions;
    using System;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using static Sundae.ErrorStanza;
    using static Sundae.PresenceStanza;

    public class XmppConnection : IDisposable
    {
        private readonly XmppStream _stream;

        private CancellationTokenSource _tokenSource;

        public XmppConnection(string host, int port) : this(host, port, host) { }

        public XmppConnection(string host, int port, string domain)
        {
            Host = host;
            Port = port;
            Domain = domain;
            _stream = new XmppStream();
        }

        public event EventHandler<XmlElement> OnElement;

        public event EventHandler<Exception> OnException;

        public event EventHandler<PresenceStanza> OnPresence;

        public event EventHandler<ErrorStanza> OnError;

        internal string Host { get; private set; }

        internal int Port { get; private set; }

        internal string Domain { get; private set; }

        public void Connect()
        {
            _stream.Connect(Host, Port, Domain);
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

            // Raising the XmlElement event.
            OnElement?.Invoke(this, element);

            // Raising the proper stanza event.
            var raised =
                Raise(GetError(element), OnError) ||
                Raise(GetPresence(element), OnPresence);
        }

        private void _Disconnect(bool disconnectStream)
        {
            _tokenSource.Cancel();
            _tokenSource.Dispose();

            if (disconnectStream)
                _stream.Disconnect();
        }

        private bool Raise<T>(T e, EventHandler<T> handler)
        {
            if (e == null)
                return false;

            handler?.Invoke(this, e);
            return true;
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
    }
}