namespace Sundae
{
    using System.Xml;
    using static ErrorElement;

    public class IqStanza
    {
        internal IqStanza() { }

        public string Id { get; set; }

        public string Result { get; set; }

        public XmlElement Element { get; set; }

        public ErrorElement Error { get; set; }

        internal static IqStanza GetIq(XmlElement element)
        {
            if (element.Name != "iq")
                return null;

            return new IqStanza
            {
                Id = element.GetAttributeOrThrow("id"),
                Result = element.GetAttributeOrThrow("result"),
                Element = element,
                Error = element.GetAttribute("type") == "error" ? GetStanzaError(element.SingleChild("error")) : null,
            };
        }
    }
}