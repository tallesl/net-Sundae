namespace Sundae.Stanzas
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Linq;

    public class ErrorStanza
    {
        public string DefinedCondition { get; set; }

        public string Text { get; set; }

        public XmlElement Xml { get; set; }

        public static bool TryGetError(XmlElement element, out ErrorStanza error)
        {
            var errorElement = GetStreamError(element) ?? GetStanzaError(element);

            if (errorElement == null)
            {
                error = null;
                return false;
            }

            var children = errorElement.ChildNodes.Cast<XmlElement>();

            var definedConditions = children.Where(e => e.Name != "text");
            var texts = children.Where(e => e.Name == "text");

            if (!definedConditions.Any())
                throw new UnexpectedXmlException("No defined condition element found:", element);

            if (definedConditions.Count() > 1)
                throw new UnexpectedXmlException("Multiple defined conditions found:", element);

            if (texts.Count() > 1)
                throw new UnexpectedXmlException("Multiple text elements found:", element);

            error = new ErrorStanza
            {
                DefinedCondition = definedConditions.Single().Name,
                Text = texts.SingleOrDefault()?.InnerText.Trim(),
                Xml = element,
            };

            return true;
        }

        private static XmlElement GetStreamError(XmlElement element) => element.Name == "stream:error" ? element : null;

        private static XmlElement GetStanzaError(XmlElement element)
        {
            if (element.GetAttribute("type") != "error")
                return null;

           var  elements = element.ChildNodes.Cast<XmlElement>().Where(e => e.Name == "error");

            if (!elements.Any())
                throw new UnexpectedXmlException("Found a stanza of type error without error element:", element);

            if (elements.Count() > 1)
                throw new UnexpectedXmlException("Multiple error elements found:", element);

            return elements.Single();
        }
    }
}