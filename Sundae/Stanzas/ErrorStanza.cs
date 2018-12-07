namespace Sundae
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Linq;

    public class ErrorStanza
    {
        internal ErrorStanza() { }

        public string DefinedCondition { get; set; }

        public string Text { get; set; }

        public XmlElement Element { get; set; }

        public static bool TryGetError(XmlElement element, out ErrorStanza error)
        {
            var errorElement = GetStreamError(element) ?? GetStanzaError(element);

            if (errorElement == null)
            {
                error = null;
                return false;
            }

            var children = errorElement.Children();

            var definedConditions = children.Where(e => e.Name != "text");

            if (!definedConditions.Any())
                throw new UnexpectedXmlException("No defined condition element found:", element);

            if (definedConditions.Count() > 1)
                throw new UnexpectedXmlException("Multiple defined conditions found:", element);

            error = new ErrorStanza
            {
                DefinedCondition = definedConditions.Single().Name,
                Text = errorElement.SingleChildOrDefault("text")?.InnerText.Trim(),
                Element = element,
            };

            return true;
        }

        private static XmlElement GetStreamError(XmlElement element) =>
            element.Name == "stream:error" ? element : null;

        private static XmlElement GetStanzaError(XmlElement element) =>
            element.GetAttribute("type") == "error" ? element.SingleChild("error") : null;
    }
}