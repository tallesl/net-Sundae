namespace Sundae
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Linq;

    public class PresenceStanza
    {
        internal PresenceStanza() { }

        public bool Available => string.IsNullOrEmpty(Type);

        public string Jid { get; set; }

        public string Type { get; set; }

        public string Show { get; set; }

        public string Status { get; set; }

        public XmlElement Element { get; set; }

        internal static PresenceStanza GetPresence(XmlElement element) =>
            element.Name != "presence" || element.GetAttribute("type") == "error" ?
            null :
            new PresenceStanza
            {
                Jid = element.GetAttribute("from"),
                Type = element.GetAttribute("type"),
                Show = element.SingleChildOrDefault("show")?.InnerText.Trim(),
                Status = element.SingleChildOrDefault("status")?.InnerText.Trim(),
                Element = element,
            };
    }
}