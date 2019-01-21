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

        internal static ErrorStanza GetError(XmlElement element)
        {
            if (element.Name != "error" && element.Name != "stream:error")
                return null;

            return new ErrorStanza
            {
                DefinedCondition = GetDefinedCondition(element),
                Text = element.SingleChildOrDefault("text")?.InnerText.Trim(),
                Element = element,
            };
        }

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