namespace Sundae
{
    using Exceptions;

    using System.Collections.Concurrent;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using System.Threading;
    using System.Xml;
    using System;
    using static Sundae.ErrorStanza;
    using static Sundae.IqStanza;
    using static Sundae.MessageStanza;
    using static Sundae.PresenceStanza;

    public class XmppConnection : IDisposable
    {
        private readonly XmppStream _stream;

        private readonly KeyedWait<string, XmlElement> _pendingIq;

        private CancellationTokenSource _tokenSource;

        public XmppConnection(string host, int port) : this(host, port, host) { }

        public XmppConnection(string host, int port, string domain)
        {
            Host = host;
            Port = port;
            Domain = domain;

            _stream = new XmppStream();
            _pendingIq = new KeyedWait<string, XmlElement>();
        }

        public event EventHandler<ErrorStanza> OnStreamError;

        public event EventHandler<MessageStanza> OnMessage;

        public event EventHandler<PresenceStanza> OnPresence;

        public event EventHandler<IqStanza> OnIq;

        public event EventHandler<XmlElement> OnElement;

        public event EventHandler<Exception> OnException;

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

        public void SendCustom(XmlElement data) => SendCustom(data.OuterXml);

        public XmlElement SendCustom(string id, string data, int? millisecondsTimeout = null)
        {
            var element = _pendingIq.Get(id, millisecondsTimeout);

            SendCustom(data);

            return element.Result;
        }

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
                Raise(GetError(element), OnStreamError) ||
                Raise(GetMessage(element), OnMessage) ||
                Raise(GetPresence(element), OnPresence);
                Raise(GetIq(element), OnIq);
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

            RunTask(() => handler?.Invoke(this, e));

            var iq = e as IqStanza;

            if (iq != null)
                _pendingIq.Set(iq.Id, iq.Element);

            return true;
        }

        private void RunTask(Action action, CancellationToken? loop = null)
        {
            Task.Run(() =>
            {
                try
                {
                    do
                    {
                        action();
                    }
                    while (loop.HasValue && !loop.Value.IsCancellationRequested);
                }
                catch (Exception e)
                {
                    OnException?.Invoke(this, e);
                }
            });
        }
    }
}