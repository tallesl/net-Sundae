namespace Sundae
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Linq;

    /// <summary>
    /// Stream or stanza-related error given by the XMPP server.
    /// https://tools.ietf.org/html/rfc6120#section-8.3
    /// </summary>
    public class StanzaError
    {
        internal StanzaError() { }

        /// <summary>
        /// Type attribute of this stanza.
        /// Refer to the enumeration documentation for description and possible values.
        /// </summary>
        public string Type { get; set; } // TODO StanzaErrorType

        /// <summary>
        /// Predefined conditions for stanza and stream-level errors.
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3
        /// </summary>
        public string DefinedCondition { get; set; } // TODO StanzaErrorDefinedCondition

        /// <summary>
        /// Descriptive or diagnostic information that supplements the meaning of a defined condition or
        /// application-specific condition.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// XML element of this error.
        /// </summary>
        public XmlElement Element { get; set; }

        internal static StanzaError GetStanzaError(XmlElement element)
        {
            if (element.GetAttribute("type") != "error")
                return null;

            var errorElement = element.SingleChild("error");

            if (errorElement.Name != "error")
                throw new UnexpectedXmlException("Missing \"error\" element:", element);

            return new StanzaError
            {
                Type = errorElement.GetAttribute("type"),
                DefinedCondition = errorElement.GetDefinedCondition(),
                Text = errorElement.SingleChildOrDefault("text")?.InnerText.Trim(),
                Element = errorElement,
            };
        }
    }
}