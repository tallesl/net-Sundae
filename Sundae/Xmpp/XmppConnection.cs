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
    using static Sundae.ErrorElement;
    using static Sundae.IqStanza;
    using static Sundae.MessageStanza;
    using static Sundae.PresenceStanza;

    public class XmppConnection : IDisposable
    {
        private readonly XmppStream _stream;

        private readonly KeyedWait<string, XmlElement> _pendingIq;

        private CancellationTokenSource _tokenSource;

        private int _id;

        public XmppConnection(string host, int port) : this(host, port, host) { }

        public XmppConnection(string host, int port, string domain)
        {
            Host = host;
            Port = port;
            Domain = domain;

            _stream = new XmppStream();
            _pendingIq = new KeyedWait<string, XmlElement>();
        }

        public event EventHandler<ErrorElement> OnStreamError;

        public event EventHandler<MessageStanza> OnMessage;

        public event EventHandler<PresenceStanza> OnPresence;

        public event EventHandler<XmlElement> OnElement;

        public event EventHandler<Exception> OnException;

        public event EventHandler<Exception> OnInternalException;

        internal string Host { get; private set; }

        internal int Port { get; private set; }

        internal string Domain { get; private set; }

        public void Connect()
        {
            _stream.Connect(Host, Port, Domain);
            _tokenSource = new CancellationTokenSource();

            var token = _tokenSource.Token;

            // Reading the XML stream until cancellation is requested, catching any internal exception.
            Task.Run(() =>
            {
                try
                {
                    while (!token.IsCancellationRequested)
                        Read();
                }
                catch (Exception e)
                {
                    OnInternalException?.Invoke(this, e);
                }
            });
        }

        public void Dispose() => Disconnect();

        public void Disconnect()
        {
            _tokenSource.Cancel();
            _tokenSource.Dispose();

            _pendingIq.Dispose();
            _stream.Dispose();
        }

        public void SendCustom(string data) => _stream.Write(data);

        public void SendCustom(XmlElement data) => SendCustom(data.OuterXml);

        public Task<XmlElement> SendCustom(string id, string data, int? millisecondsTimeout = null)
        {
            // Sets up the blocking call.
            var element = _pendingIq.Get(id, millisecondsTimeout);

            // Send the data.
            SendCustom(data);

            // Future for the element.
            return element;
        }

        internal string NextId() => Interlocked.Increment(ref _id).ToString();

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
                Disconnect();
                return;
            }

            // Always raise the XmlElement event.
            RaiseEvent(element, OnElement);

            // Raising the proper stanza event.
            var _ =
                UnblockIqCall(GetIq(element)) ||
                RaiseEvent(GetStreamError(element), OnStreamError) ||
                RaiseEvent(GetMessage(element), OnMessage) ||
                RaiseEvent(GetPresence(element), OnPresence);
        }

        private bool UnblockIqCall(IqStanza iq)
        {
            if (iq == null)
                return false;

            // Unblocks any blocking call waiting for the IQ response.
            _pendingIq.Set(iq.Id, iq.Element);

            return true;
        }

        private bool RaiseEvent<T>(T e, EventHandler<T> handler)
        {
            // Checking if the event should be handled by this call.
            // Enables this method to be called in a pipeline fashion.
            if (e == null)
                return false;

            // Runs the user event handler on another thread, catching any exception on their part.
            Task.Run(() =>
            {
                try
                {
                    handler?.Invoke(this, e);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, ex);
                }
            });

            // Returning that the event was handled by this call.
            // Enables this method to be called in a pipeline fashion.
            return true;
        }
    }
}