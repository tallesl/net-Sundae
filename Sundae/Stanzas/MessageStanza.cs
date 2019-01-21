namespace Sundae
{
    using System.Xml;
    using static ErrorStanza;

    public class MessageStanza
    {
        internal MessageStanza() { }

        public string From { get; set; }

        public string To { get; set; }

        public string Type { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string Thread { get; set; }

        public ErrorStanza Error { get; set; }

        public XmlElement Element { get; set; }

        internal static MessageStanza GetMessage(XmlElement element)
        {
            if (element.Name != "message")
                return null;

            return new MessageStanza
            {
                From = element.GetAttributeOrThrow("from"),
                To = element.GetAttributeOrThrow("to"),
                Type = element.GetAttribute("type"),
                Subject = element.SingleChildOrDefault("subject")?.InnerText.Trim(),
                Body = element.SingleChildOrDefault("body")?.InnerText.Trim(),
                Thread = element.SingleChildOrDefault("thread")?.InnerText.Trim(),
                Error = element.GetAttribute("type") == "error" ? GetError(element.SingleChild("error")) : null,
                Element = element,
            };
        }
    }
}