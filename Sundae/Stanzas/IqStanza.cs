namespace Sundae
{
    using System.Xml;
    using static StanzaError;

    /// <summary>
    /// IQ (Info/Query) stanza given by the XMPP server.
    /// https://tools.ietf.org/html/rfc6120#section-8.2.3
    /// </summary>
    public class IqStanza
    {
        internal IqStanza() { }

        /// <summary>
        /// JID of the intended recipient.
        /// https://tools.ietf.org/html/rfc6120#section-8.1.1
        /// </summary>
        public Jid To { get; private set; }

        /// <summary>
        /// JID of the sender.
        /// https://tools.ietf.org/html/rfc6120#section-8.1.2
        /// </summary>
        public Jid From { get; private set; }

        /// <summary>
        /// Used by the originating entity to track any response or error stanza that it might receive in relation to
        /// the generated stanza from another entity.
        /// https://tools.ietf.org/html/rfc6120#section-8.1.3
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Type attribute of this stanza.
        /// Refer to the enumeration documentation for description and possible values.
        /// https://tools.ietf.org/html/rfc6120#section-8.1.4
        /// https://tools.ietf.org/html/rfc6120#section-8.2.3
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// XML element of this stanza.
        /// </summary>
        public XmlElement Element { get; private set; }

        /// <summary>
        /// Stanza-related error, if any.
        /// </summary>
        public StanzaError Error { get; private set; }

        internal static IqStanza GetIq(XmlElement element)
        {
            if (element.Name != "iq")
                return null;

            return new IqStanza
            {
                Id = element.GetAttributeOrThrow("id"),
                Type = element.GetAttributeOrThrow("type"),
                Element = element,
                Error = GetStanzaError(element),
            };
        }
    }
}