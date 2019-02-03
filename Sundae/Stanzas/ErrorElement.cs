namespace Sundae
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Linq;

    /// <summary>
    /// Stream or stanza-related error given by the XMPP server.
    /// https://tools.ietf.org/html/rfc6120#section-4.9
    /// https://tools.ietf.org/html/rfc6120#section-8.3
    /// </summary>
    public class ErrorElement
    {
        internal ErrorElement() { }

        // TODO replace ErrorElement with StreamError and StanzaError (defined conditions are different)

        /// <summary>
        /// Predefined conditions for stanza and stream-level errors.
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3
        /// </summary>
        public string DefinedCondition { get; set; } // TODO StreamErrorDefinedCondition enum and StanzaErrorDefinedCondition enum

        /// <summary>
        /// Descriptive or diagnostic information that supplements the meaning of a defined condition or
        /// application-specific condition.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// XML element of this error element.
        /// </summary>
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