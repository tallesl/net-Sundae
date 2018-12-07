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

        public static bool TryGetPresence(XmlElement element, out PresenceStanza presence)
        {
            var type = element.GetAttribute("type");

            if (element.Name != "presence" || type == "error")
            {
                presence = null;
                return false;
            }

            presence = new PresenceStanza
            {
                Jid = element.GetAttribute("from"),
                Type = type,
                Show = element.SingleChildOrDefault("show")?.InnerText.Trim(),
                Status = element.SingleChildOrDefault("status")?.InnerText.Trim(),
                Element = element,
            };

            return true;
        }
    }
}