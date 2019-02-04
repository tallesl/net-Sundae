namespace Sundae
{
    using System.Xml;
    using static ErrorElement;

    public class IqStanza
    {
        internal IqStanza() { }

        public string Id { get; set; }

        /// <summary>
        /// Type attribute of this stanza.
        /// Refer to the enumeration documentation for description and possible values.
        /// https://tools.ietf.org/html/rfc6120#section-8.1.4
        /// https://tools.ietf.org/html/rfc6120#section-8.2.3
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// XML element of this stanza.
        /// </summary>
        public XmlElement Element { get; set; }

        public ErrorElement Error { get; set; }

        internal static IqStanza GetIq(XmlElement element)
        {
            if (element.Name != "iq")
                return null;

            return new IqStanza
            {
                Id = element.GetAttributeOrThrow("id"),
                Type = element.GetAttributeOrThrow("type"),
                Element = element,
                Error = element.GetAttribute("type") == "error" ? GetStanzaError(element.SingleChild("error")) : null,
            };
        }
    }
}