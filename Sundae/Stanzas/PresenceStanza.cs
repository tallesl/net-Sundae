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

            var jid = element.GetAttribute("from");

            if (string.IsNullOrEmpty(jid))
                throw new UnexpectedXmlException("Missing \"from\" attribute:", element);

            var children = element.ChildNodes.Cast<XmlElement>();
            var shows = children.Where(e => e.Name == "show");
            var statuses = children.Where(e => e.Name == "status");

            if (shows.Count() > 1)
                throw new UnexpectedXmlException("Multiple \"show\" element found:", element);

            if (statuses.Count() > 1)
                throw new UnexpectedXmlException("Multiple \"status\" element found:", element);

            var show = shows.SingleOrDefault()?.InnerText.Trim() ?? string.Empty;
            var status = statuses.SingleOrDefault()?.InnerText.Trim() ?? string.Empty;

            presence = new PresenceStanza
            {
                Jid = jid,
                Type = type,
                Show = show,
                Status = status,
                Element = element,
            };

            return true;
        }
    }
}