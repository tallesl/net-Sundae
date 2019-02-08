namespace Sundae
{
    /// <summary>
    /// Presence stanza types.
    /// https://tools.ietf.org/html/rfc6121#section-4.7.1
    /// </summary>
    public enum PresenceType
    {
        /// <summary>
        /// An error has occurred regarding processing of a previously sent presence stanza.
        /// </summary>
        Error,

        /// <summary>
        /// A request for an entity's current presence; SHOULD be generated only by a server on behalf of a user.
        /// </summary>
        Probe,

        /// <summary>
        /// The sender wishes to subscribe to the recipient's presence.
        /// </summary>
        Subscribe,

        /// <summary>
        /// The sender has allowed the recipient to receive their presence.
        /// </summary>
        Subscribed,

        /// <summary>
        /// The sender is no longer available for communication.
        /// </summary>
        Unavailable,

        /// <summary>
        /// The sender is unsubscribing from the receiver's presence.
        /// </summary>
        Unsubscribe,

        /// <summary>
        /// The subscription request has been denied or a previously granted subscription has been canceled.
        /// </summary>
        Unsubscribed,

        /// <summary>
        /// A value not listed in the RFC was found.
        /// Please refer to the XML element for the actual value.
        /// https://tools.ietf.org/html/rfc6121#section-4.7.1
        /// </summary>
        Unknown,
    }
}