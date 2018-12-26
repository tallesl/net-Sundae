namespace Sundae
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Linq;
    using static ErrorStanza;

    public class PresenceStanza
    {
        internal PresenceStanza() { }

        // https://tools.ietf.org/html/rfc6121#section-4.7.1
        // The absence of a 'type' attribute signals that the relevant entity is available for communication.
        public bool AvailableForCommunication => string.IsNullOrEmpty(Type);

        public string Jid { get; set; }

        public string Type { get; set; }

        public string Show { get; set; }

        public string Status { get; set; }

        public ErrorStanza Error { get; set; }

        public XmlElement Element { get; set; }

        internal static PresenceStanza GetPresence(XmlElement element) =>
            element.Name == "presence" ?
            new PresenceStanza
            {
                Jid = element.GetAttribute("from"),
                Type = element.GetAttribute("type"),
                Show = element.SingleChildOrDefault("show")?.InnerText.Trim(),
                Status = element.SingleChildOrDefault("status")?.InnerText.Trim(),
                Error = element.GetAttribute("type") == "error" ? GetError(element.SingleChild("error")) : null,
                Element = element,
            } : null;
    }
}