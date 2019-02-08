namespace Sundae
{
    /// <summary>
    /// Stanza error types.
    /// https://tools.ietf.org/html/rfc6120#section-8.3.2
    /// </summary>
    public enum StanzaErrorType
    {
        /// <summary>
        /// Retry after providing credentials.
        /// </summary>
        Auth,

        /// <summary>
        /// Do not retry (the error cannot be remedied).
        /// </summary>
        Cancel,

        /// <summary>
        /// Proceed (the condition was only a warning).
        /// </summary>
        Continue,

        /// <summary>
        /// Retry after changing the data sent.
        /// </summary>
        Modify,

        /// <summary>
        /// Retry after waiting (the error is temporary).
        /// </summary>
        Wait,

        /// <summary>
        /// A value not listed in the RFC was found.
        /// Please refer to the XML element for the actual value.
        /// https://tools.ietf.org/html/rfc6120#section-8.3.2
        /// </summary>
        Unknown,
    }
}