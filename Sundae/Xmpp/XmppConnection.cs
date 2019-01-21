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
            // Sets up the blocking call.
            var element = _pendingIq.Get(id, millisecondsTimeout);

            // Send the data.
            SendCustom(data);

            // Blocks.
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
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
            _tokenSource = null;

            if (disconnectStream)
                _stream.Disconnect();
        }

        private bool Raise<T>(T e, EventHandler<T> handler)
        {
            // Checking if the event should be handled by this call.
            // Enables this method to be called in a pipeline fashion.
            if (e == null)
                return false;

            // Runs the user event handler asynchronously catching any exception on their part.
            RunTask(() => handler?.Invoke(this, e));

            var iq = e as IqStanza;

            // Unblocks any blocking call waiting for the IQ response.
            if (iq != null)
                _pendingIq.Set(iq.Id, iq.Element);

            // Returning that the event was handled by this call.
            // Enables this method to be called in a pipeline fashion.
            return true;
        }

        private void RunTask(Action action, CancellationToken? loop = null)
        {
            // Runs the given action catching any eventual exception and raising the proper event.
            Task.Run(() =>
            {
                try
                {
                    do
                    {
                        action();
                    }
                    // The cancellation token acts as both a flag for running as a loop (HasValue) and its condition to
                    // stop (IsCancellationRequested).
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