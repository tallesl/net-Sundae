namespace Sundae
{
    /// <summary>
    /// IQ (Info/Query) stanza types.
    /// https://tools.ietf.org/html/rfc6120#section-8.2.3
    /// </summary>
    public enum IqType
    {
        /// <summary>
        /// A value not listed in the RFC was found.
        /// Please refer to the XML element for the actual value.
        /// https://tools.ietf.org/html/rfc6120#section-8.2.3
        /// </summary>
        Unknown,

        /// <summary>
        /// The stanza requests information, inquires about what data is needed in order to complete further operations,
        /// etc.
        /// </summary>
        Get,

        /// <summary>
        /// The stanza provides data that is needed for an operation to be completed, sets new values, replaces existing
        /// values, etc.
        /// </summary>
        Set,

        /// <summary>
        /// The stanza is a response to a successful get or set request.
        /// </summary>
        Result,

        /// <summary>
        /// The stanza reports an error that has occurred regarding processing or delivery of a get or set request.
        /// </summary>
        Error,
    }
}