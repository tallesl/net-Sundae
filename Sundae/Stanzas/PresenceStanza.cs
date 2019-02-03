namespace Sundae
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Linq;
    using static ErrorElement;

    /// <summary>
    /// Presence stanza given by the XMPP server.
    /// https://tools.ietf.org/html/rfc6121#section-4.7
    /// </summary>
    public class PresenceStanza
    {
        internal PresenceStanza() { }

        /// <summary>
        /// The absence of a 'type' attribute signals that the relevant entity is available for communication.
        /// https://tools.ietf.org/html/rfc6121#section-4.7.1
        /// </summary>
        public bool AvailableForCommunication => string.IsNullOrEmpty(Type);

        /// <summary>
        /// JID of the intended recipient.
        /// https://tools.ietf.org/html/rfc6120#section-8.1.1
        /// </summary>
        public Jid To { get; set; }

        /// <summary>
        /// JID of the sender.
        /// https://tools.ietf.org/html/rfc6120#section-8.1.2
        /// </summary>
        public Jid From { get; set; }

        /// <summary>
        /// Used by the originating entity to track any response or error stanza that it might receive in relation to
        /// the generated stanza from another entity.
        /// https://tools.ietf.org/html/rfc6120#section-8.1.3
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Type attribute of this stanza.
        /// Refer to the enumeration documentation for description and possible values.
        /// https://tools.ietf.org/html/rfc6120#section-8.1.4
        /// </summary>
        public string Type { get; set; } // TODO PresenceType enum (https://tools.ietf.org/html/rfc6121#section-4.7.1)

        /// <summary>
        /// Show attribute of this stanza.
        /// Refer to the enumeration documentation for description and possible values.
        /// </summary>
        public string Show { get; set; }

        /// <summary>
        /// Human-readable description of an entity's availability.
        /// https://tools.ietf.org/html/rfc6121#section-4.7.2.2
        /// </summary>
        public string Status { get; set; }

        // TODO Priority property (https://tools.ietf.org/html/rfc6121#section-4.7.2.3)

        // TODO ExtendedContent property (https://tools.ietf.org/html/rfc6121#section-4.7.3)

        /// <summary>
        /// Stanza-related error, if any.
        /// </summary>
        public ErrorElement Error { get; set; }

        /// <summary>
        /// XML element of this stanza.
        /// </summary>
        public XmlElement Element { get; set; }

        internal static PresenceStanza GetPresence(XmlElement element)
        {
            if (element.Name != "presence")
                return null;

            return new PresenceStanza
            {
                From = new Jid(element.GetAttribute("from")),
                Type = element.GetAttribute("type"),
                Show = element.SingleChildOrDefault("show")?.InnerText.Trim(),
                Status = element.SingleChildOrDefault("status")?.InnerText.Trim(),
                Error = element.GetAttribute("type") == "error" ? GetStanzaError(element.SingleChild("error")) : null,
                Element = element,
            };
        }
    }
}