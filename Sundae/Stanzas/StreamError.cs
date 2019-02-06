namespace Sundae
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Linq;

    /// <summary>
    /// Stream or stanza-related error given by the XMPP server.
    /// https://tools.ietf.org/html/rfc6120#section-4.9
    /// </summary>
    public class StreamError
    {
        internal StreamError() { }

        /// <summary>
        /// Predefined conditions for stanza and stream-level errors.
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3
        /// </summary>
        public string DefinedCondition { get; set; } // TODO StreamErrorDefinedCondition

        /// <summary>
        /// Descriptive or diagnostic information that supplements the meaning of a defined condition or
        /// application-specific condition.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// XML element of this error.
        /// </summary>
        public XmlElement Element { get; set; }

        internal static StreamError GetStreamError(XmlElement element)
        {
            if (element.Name != "stream:error")
                return null;

            return new StreamError
            {
                DefinedCondition = element.GetDefinedCondition(),
                Text = element.SingleChildOrDefault("text")?.InnerText.Trim(),
                Element = element,
            };
        }
    }
}