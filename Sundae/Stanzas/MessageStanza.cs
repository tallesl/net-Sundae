namespace Sundae
{
    using System.Xml;
    using static ErrorElement;

    public class MessageStanza
    {
        internal MessageStanza() { }

        public Jid From { get; set; }

        public Jid To { get; set; }

        public string Type { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string Thread { get; set; }

        public ErrorElement Error { get; set; }

        public XmlElement Element { get; set; }

        internal static MessageStanza GetMessage(XmlElement element)
        {
            if (element.Name != "message")
                return null;

            return new MessageStanza
            {
                From = new Jid(element.GetAttributeOrThrow("from")),
                To = new Jid(element.GetAttributeOrThrow("to")),
                Type = element.GetAttribute("type"),
                Subject = element.SingleChildOrDefault("subject")?.InnerText.Trim(),
                Body = element.SingleChildOrDefault("body")?.InnerText.Trim(),
                Thread = element.SingleChildOrDefault("thread")?.InnerText.Trim(),
                Error = element.GetAttribute("type") == "error" ? GetStanzaError(element.SingleChild("error")) : null,
                Element = element,
            };
        }
    }
}