namespace Sundae
{
    using System.Xml;

    public class IqStanza
    {
        internal IqStanza() { }

        public string Id { get; set; }

        public string Result { get; set; }

        public XmlElement Element { get; set; }

        internal static IqStanza GetIq(XmlElement element) =>
            element.Name != "iq" || element.GetAttribute("type") == "error" ?
            null :
            new IqStanza
            {
                Id = element.GetAttributeOrThrow("id"),
                Result = element.GetAttributeOrThrow("result"),
                Element = element,
            };
    }
}