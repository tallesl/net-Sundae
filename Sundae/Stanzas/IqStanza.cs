namespace Sundae
{
    using System.Xml;
    using static ErrorStanza;

    public class IqStanza
    {
        internal IqStanza() { }

        public string Id { get; set; }

        public string Result { get; set; }

        public XmlElement Element { get; set; }

        public ErrorStanza Error { get; set; }

        internal static IqStanza GetIq(XmlElement element) =>
            element.Name == "iq" ?
            new IqStanza
            {
                Id = element.GetAttributeOrThrow("id"),
                Result = element.GetAttributeOrThrow("result"),
                Element = element,
                Error = element.GetAttribute("type") == "error" ? GetError(element.SingleChild("error")) : null,
            } : null;
    }
}