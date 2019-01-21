namespace Sundae
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Linq;

    public class ErrorElement
    {
        internal ErrorElement() { }

        public string DefinedCondition { get; set; }

        public string Text { get; set; }

        public XmlElement Element { get; set; }

        internal static ErrorElement GetStanzaError(XmlElement element) =>
            element.Name == "error" ? GetError(element) : null;

        internal static ErrorElement GetStreamError(XmlElement element) =>
            element.Name == "stream:error" ? GetError(element) : null;

        private static ErrorElement GetError(XmlElement errorElement) =>
            new ErrorElement
            {
                DefinedCondition = GetDefinedCondition(errorElement),
                Text = errorElement.SingleChildOrDefault("text")?.InnerText.Trim(),
                Element = errorElement,
            };

        private static string GetDefinedCondition(XmlElement errorElement)
        {
            var definedConditions = errorElement.Children().Where(e => e.Name != "text");

            if (!definedConditions.Any())
                throw new UnexpectedXmlException("No defined condition element found:", errorElement);

            if (definedConditions.Count() > 1)
                throw new UnexpectedXmlException("Multiple defined conditions found:", errorElement);

            return definedConditions.Single().Name;
        }
    }
}