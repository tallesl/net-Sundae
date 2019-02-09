namespace Sundae
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Linq;
    using static Jid;
    using static StanzaError;

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
        public bool AvailableForCommunication => !Type.HasValue;

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
        /// </summary>
        public PresenceType? Type { get; private set; }

        /// <summary>
        /// Show attribute of this stanza.
        /// Refer to the enumeration documentation for description and possible values.
        /// </summary>
        public string Show { get; private set; }

        /// <summary>
        /// Human-readable description of an entity's availability.
        /// https://tools.ietf.org/html/rfc6121#section-4.7.2.2
        /// </summary>
        public string Status { get; private set; }

        /// <summary>
        /// Integer between -128 and +127 that specifies the priority level of the resource.
        /// https://tools.ietf.org/html/rfc6121#section-4.7.2.3
        /// </summary>
        public int? Priority { get; private set; }

        /// <summary>
        /// Stanza-related error, if any.
        /// </summary>
        public StanzaError Error { get; private set; }

        /// <summary>
        /// XML element of this stanza.
        /// </summary>
        public XmlElement Element { get; private set; }

        internal static PresenceStanza GetPresence(XmlElement element)
        {
            if (element.Name != "presence")
                return null;

            return new PresenceStanza
            {
                From = GetJid(element, "from"),
                Type = element.GetAttribute("type").ToEnum<PresenceType?>(),
                Show = element.SingleChildOrDefault("show")?.InnerText.Trim(),
                Status = element.SingleChildOrDefault("status")?.InnerText.Trim(),
                Priority = element.GetIntegerAttribute("priority"),
                Error = GetStanzaError(element),
                Element = element,
            };
        }
    }
}