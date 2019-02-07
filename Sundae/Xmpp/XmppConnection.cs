namespace Sundae
{
    using System.Collections.Concurrent;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using System.Threading;
    using System.Xml;
    using System;
    using static IqStanza;
    using static MessageStanza;
    using static PresenceStanza;
    using static StreamError;

    /// <summary>
    /// Connection to XMPP server.
    /// </summary>
    public class XmppConnection : IDisposable
    {
        private readonly XmppStream _stream;

        private readonly KeyedWait<string, XmlElement> _pendingResults;

        private CancellationTokenSource _tokenSource;

        private int _id;

        /// <summary>
        /// Creates a XMPP connection.
        /// Assumes 5222 for the port and the given host for the XMPP domain.
        /// </summary>
        /// <param name="host">Host to connect to</param>
        public XmppConnection(string host) : this(host, 5222, host) { }

        /// <summary>
        /// Creates a XMPP connection.
        /// Assumes the given host for the XMPP domain.
        /// </summary>
        /// <param name="host">Host to connect to</param>
        /// <param name="port">Port to connect to</param>
        public XmppConnection(string host, int port) : this(host, port, host) { }

        /// <summary>
        /// Creates a XMPP connection.
        /// </summary>
        /// <param name="host">Host to connect to</param>
        /// <param name="port">Port to connect to</param>
        /// <param name="domain">XMPP domain to use</param>
        public XmppConnection(string host, int port, string domain)
        {
            Host = host;
            Port = port;
            Domain = domain;

            _stream = new XmppStream();
            _pendingResults = new KeyedWait<string, XmlElement>();
        }

        /// <summary>
        /// The server ended up the stream with an error.
        /// The connection is diposed and can't be further used after this event is raised.
        /// </summary>
        public event EventHandler<StreamError> OnStreamError;

        /// <summary>
        /// A message stanza was received.
        /// </summary>
        public event EventHandler<MessageStanza> OnMessage;

        /// <summary>
        /// A presence stanza was received.
        /// </summary>
        public event EventHandler<PresenceStanza> OnPresence;

        /// <summary>
        /// A XML element is received by the connection.
        /// This event is raised before any further processing.
        /// </summary>
        public event EventHandler<XmlElement> OnElement;

        /// <summary>
        /// One of the provided event handlers throwed an exception (make sure this one doesn't throw, else the
        /// exception is swallowed).
        /// </summary>
        public event EventHandler<Exception> OnException;

        /// <summary>
        /// Something bad happened internally.
        /// </summary>
        public event EventHandler<Exception> OnInternalException;

        /// <summary>
        /// Host to connect to.
        /// </summary>
        public string Host { get; private set; }

        /// <summary>
        /// Port to connect to.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// XMPP domain to use.
        /// </summary>
        public string Domain { get; private set; }

        /// <summary>
        /// Connects to server.
        /// </summary>
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

        /// <summary>
        /// Disconnects from the server and disposes this object.
        /// </summary>
        public void Dispose() => Dispose(null);

        /// <summary>
        /// Sends a custom XML of yours to the XMPP server.
        /// </summary>
        /// <param name="xml">Custom XML to send</param>
        public void SendCustom(string xml) => _stream.Write(xml);

        /// <summary>
        /// Sends a custom XML of yours to the server.
        /// </summary>
        /// <param name="xml">Custom XML to send</param>
        public void SendCustom(XmlElement xml) => SendCustom(xml.OuterXml);

        /// <summary>
        /// Sends a custom XML of yours returning a task with the resulted stanza given by the server.
        /// Setting up the id attribute is optional (a random string is used if none is provided).
        /// </summary>
        /// <param name="xml">Custom XML to send</param>
        /// <param name="timeout">Timeout of the request in milliseconds (null to wait indefinitely)</param>
        /// <returns>The resulted stanza given by the server</returns>
        public Task<XmlElement> SendCustomWithResult(string xml, int? timeout = null) =>
            SendCustomWithResult(xml.ToXmlElement(), timeout);

        /// <summary>
        /// Sends a custom XML of yours returning a task with the resulted stanza given by the server.
        /// Setting up the id attribute is optional (a random string is used if none is provided).
        /// </summary>
        /// <param name="xml">Custom XML to send</param>
        /// <param name="timeout">Timeout of the request in milliseconds (null to wait indefinitely)</param>
        /// <returns>The resulted stanza given by the server</returns>
        public Task<XmlElement> SendCustomWithResult(XmlElement xml, int? timeout = null)
        {
            // Setting an id if not provided.
            if (!xml.HasAttribute("id"))
                xml.SetAttribute("id", NextId());

            // Sets up the blocking call.
            var id = xml.GetAttribute("id");
            var result = _pendingResults.Get(id, timeout);

            // Send the data.
            SendCustom(xml);

            // Future for the element.
            return result;
        }

        private void Dispose(Exception innerDispose)
        {
            _tokenSource.Cancel();
            _tokenSource.Dispose();

            _pendingResults.Dispose(innerDispose);
            _stream.Dispose(innerDispose);
        }

        private void Read()
        {
            XmlElement element;

            try
            {
                // Getting next element on stream (blocking call).
                element = _stream.Read();
            }
            catch (XmlStreamClosedException e)
            {
                // Disconnecting if got a </stream:stream>.
                Dispose(e);
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
            _pendingResults.Set(iq.Id, iq.Element);

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
                    try
                    {
                        OnException?.Invoke(this, ex);
                    }
                    catch { }
                }
            });

            // Returning that the event was handled by this call.
            // Enables this method to be called in a pipeline fashion.
            return true;
        }

        private string NextId() => Interlocked.Increment(ref _id).ToString();
    }
}