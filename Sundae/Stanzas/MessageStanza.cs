namespace Sundae
{
    using System.Xml;

    public class MessageStanza
    {
        internal MessageStanza() { }

        public string From { get; set; }

        public string To { get; set; }

        public string Type { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string Thread { get; set; }

        public XmlElement Element { get; set; }

        internal static MessageStanza GetMessage(XmlElement element) =>
            element.Name != "presence" || element.GetAttribute("type") == "error" ?
            null :
            new MessageStanza
            {
                From = element.GetAttributeOrThrow("from"),
                To = element.GetAttributeOrThrow("to"),
                Type = element.GetAttribute("type"),
                Subject = element.SingleChildOrDefault("subject")?.InnerText.Trim(),
                Body = element.SingleChildOrDefault("body")?.InnerText.Trim(),
                Thread = element.SingleChildOrDefault("thread")?.InnerText.Trim(),
                Element = element,
            };
    }
}