namespace Sundae
{
    /// <summary>
    /// Conditions used in stanza errors.
    /// https://tools.ietf.org/html/rfc6120#section-8.3.3
    /// </summary>
    public enum StanzaErrorCondition
    {
        /// <summary>
        /// The sender has sent a stanza containing XML that does not conform to the appropriate schema or that cannot
        /// be processed (e.g., an IQ stanza that includes an unrecognized value of the 'type' attribute, or an element
        /// that is qualified by a recognized namespace but that violates the defined syntax for the element).
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3.1
        /// </summary>
        BadRequest,

        /// <summary>
        /// Access cannot be granted because an existing resource exists with the same name or address.
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3.2
        /// </summary>
        Conflict,

        /// <summary>
        /// The feature represented in the XML stanza is not implemented by the intended recipient or an intermediate
        /// server and therefore the stanza cannot be processed (e.g., the entity understands the namespace but does not
        /// recognize the element name).
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3.3
        /// </summary>
        FeatureNotImplemented,

        /// <summary>
        /// The requesting entity does not possess the necessary permissions to perform an action that only certain
        /// authorized roles or individuals are allowed to complete (i.e., it typically relates to authorization rather
        /// than authentication). 
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3.4
        /// </summary>
        Forbidden,

        /// <summary>
        /// The recipient or server can no longer be contacted at this address, typically on a permanent basis (as
        /// opposed to the &lt;redirect/&gt; error condition, which is used for temporary addressing failures).
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3.5
        /// </summary>
        Gone,

        /// <summary>
        /// The server has experienced a misconfiguration or other internal error that prevents it from processing the
        /// stanza.
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3.6
        /// </summary>
        InternalServerError,

        /// <summary>
        /// The addressed JID or item requested cannot be found.
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3.7
        /// </summary>
        ItemNotFound,

        /// <summary>
        /// The sending entity has provided (e.g., during resource binding) or communicated (e.g., in the 'to' address
        /// of a stanza) an XMPP address or aspect thereof that violates the rules of a XMPP address format.
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3.8
        /// </summary>
        JidMalformed,

        /// <summary>
        /// The recipient or server understands the request but cannot process it because the request does not meet
        /// criteria defined by the recipient or server (e.g., a request to subscribe to information that does not
        /// simultaneously include configuration parameters needed by the recipient). 
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3.9
        /// </summary>
        NotAcceptable,

        /// <summary>
        /// The recipient or server does not allow any entity to perform the action (e.g., sending to entities at a
        /// blacklisted domain).
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3.10
        /// </summary>
        NotAllowed,

        /// <summary>
        /// The sender needs to provide credentials before being allowed to perform the action, or has provided improper
        /// credentials (the name "not-authorized", which was borrowed from the "401 Unauthorized" error of HTTP, might
        /// lead the reader to think that this condition relates to authorization, but instead it is typically used in
        /// relation to authentication).
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3.11
        /// </summary>
        NotAuthorized,

        /// <summary>
        /// The entity has violated some local service policy (e.g., a message contains words that are prohibited by the
        /// service) and the server MAY choose to specify the policy in the &lt;text/&gt; element or in an
        /// application-specific condition element.
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3.12
        /// </summary>
        PolicyViolation,

        /// <summary>
        /// The intended recipient is temporarily unavailable, undergoing maintenance, etc.
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3.13
        /// </summary>
        RecipientUnavailable,

        /// <summary>
        /// The recipient or server is redirecting requests for this information to another entity, typically in a
        /// temporary fashion (as opposed to the &lt;gone/&gt; error condition, which is used for permanent addressing
        /// failures).
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3.14
        /// </summary>
        Redirect,

        /// <summary>
        /// The requesting entity is not authorized to access the requested service because prior registration is
        /// necessary (examples of prior registration include members-only rooms in XMPP multi-user chat and gateways to
        /// non-XMPP instant messaging services, which traditionally required registration in order to use the gateway). 
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3.15
        /// </summary>
        RegistrationRequired,

        /// <summary>
        /// A remote server or service specified as part or all of the JID of the intended recipient does not exist or
        /// cannot be resolved (e.g., there is no _xmpp-server._tcp DNS SRV record, the A or AAAA fallback resolution
        /// fails, or A/AAAA lookups succeed but there is no response on the IANA-registered port 5269).
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3.16
        /// </summary>
        RemoteServerNotFound,

        /// <summary>
        /// A remote server or service specified as part or all of the JID of the intended recipient (or needed to
        /// fulfill a request) was resolved but communications could not be established within a reasonable amount of
        /// time (e.g., an XML stream cannot be established at the resolved IP address and port, or an XML stream can be
        /// established but stream negotiation fails because of problems with TLS, SASL, Server Dialback, etc.).
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3.17
        /// </summary>
        RemoteServerTimeout,

        /// <summary>
        /// The server or recipient is busy or lacks the system resources necessary to service the request.
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3.18
        /// </summary>
        ResourceConstraint,

        /// <summary>
        /// The server or recipient does not currently provide the requested service.
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3.19
        /// </summary>
        ServiceUnavailable,

        /// <summary>
        /// The requesting entity is not authorized to access the requested service because a prior subscription is
        /// necessary.
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3.20
        /// </summary>
        SubscriptionRequired,

        /// <summary>
        /// The error condition is not one of those defined by the other conditions in this list.
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3.21
        /// </summary>
        UndefinedCondition,

        /// <summary>
        /// The recipient or server understood the request but was not expecting it at this time (e.g., the request was
        /// out of order).
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3.22
        /// </summary>
        UnexpectedRequest,

        /// <summary>
        /// A value not listed in the RFC was found.
        /// Please refer to the XML element for the actual value.
        /// https://tools.ietf.org/html/rfc6120#section-8.3.3
        /// </summary>
        Unknown,
    }
}