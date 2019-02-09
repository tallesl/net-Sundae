namespace Sundae
{
    /// <summary>
    /// Stream-level error conditions.
    /// https://tools.ietf.org/html/rfc6120#section-4.9.3
    /// </summary>
    public enum StreamErrorCondition
    {
        /// <summary>
        /// A value not listed in the RFC was found.
        /// Please refer to the XML element for the actual value.
        /// </summary>
        Unknown,

        /// <summary>
        /// The entity has sent XML that cannot be processed.
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.1
        /// </summary>
        BadFormat,

        /// <summary>
        /// The entity has sent a namespace prefix that is unsupported, or has sent no namespace prefix on an element
        /// that needs such a prefix.
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.2
        /// </summary>
        BadNamespacePrefix,

        /// <summary>
        /// The server either is closing the existing stream for this entity because a new stream has been initiated
        /// that conflicts with the existing stream, or is refusing a new stream for this entity because allowing the
        /// new stream would conflict with an existing stream.
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.3
        /// </summary>
        Conflict,

        /// <summary>
        /// One party is closing the stream because it has reason to believe that the other party has permanently lost
        /// the ability to communicate over the stream.
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.4
        /// </summary>
        ConnectionTimeout,

        /// <summary>
        /// The value of the 'to' attribute provided in the initial stream header corresponds to an FQDN that is no
        /// longer serviced by the receiving entity.
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.5
        /// </summary>
        HostGone,

        /// <summary>
        /// The value of the 'to' attribute provided in the initial stream header does not correspond to an FQDN that is
        /// serviced by the receiving entity.
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.6
        /// </summary>
        HostUnknown,

        /// <summary>
        /// A stanza sent between two servers lacks a 'to' or 'from' attribute, the 'from' or 'to' attribute has no
        /// value, or the value violates the rules for XMPP addresses.
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.7
        /// </summary>
        ImproperAddressing,

        /// <summary>
        /// The server has experienced a misconfiguration or other internal error that prevents it from servicing the
        /// stream.
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.8
        /// </summary>
        InternalServerError,

        /// <summary>
        /// The data provided in a 'from' attribute does not match an authorized JID or validated domain as negotiated
        /// between two servers using SASL or Server Dialback, or between a client and a server via SASL authentication
        /// and resource binding.
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.9
        /// </summary>
        InvalidFrom,

        /// <summary>
        /// The stream namespace name is something other than "http://etherx.jabber.org/streams" or the content
        /// namespace declared as the default namespace is not supported (e.g., something other than "jabber:client" or
        /// "jabber:server").
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.10
        /// </summary>
        InvalidNamespace,

        /// <summary>
        /// The entity has sent invalid XML over the stream to a server that performs validation.
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.11
        /// </summary>
        InvalidXml,

        /// <summary>
        /// The entity has attempted to send XML stanzas or other outbound data before the stream has been
        /// authenticated, or otherwise is not authorized to perform an action related to stream negotiation.
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.12
        /// </summary>
        NotAuthorized,

        /// <summary>
        /// The initiating entity has sent XML that is not well-formed.
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.13
        /// </summary>
        NotWellFormed,

        /// <summary>
        /// The entity has violated some local service policy.
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.14
        /// </summary>
        PolicyViolation,

        /// <summary>
        /// The server is unable to properly connect to a remote entity that is needed for authentication or
        /// authorization.
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.15
        /// </summary>
        RemoteConnectionFailed,

        /// <summary>
        /// The server is closing the stream because it has new (typically security-critical) features to offer, because
        /// the keys or certificates used to establish a secure context for the stream have expired or have been revoked
        /// during the life of the stream, because the TLS sequence number has wrapped), etc.
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.16
        /// </summary>
        Reset,

        /// <summary>
        /// The server lacks the system resources necessary to service the stream.
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.17
        /// </summary>
        ResourceConstraint,

        /// <summary>
        /// The entity has attempted to send restricted XML features such as a comment, processing instruction, DTD
        /// subset, or XML entity reference.
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.18
        /// </summary>
        RestrictedXml,


        /// <summary>
        /// The server will not provide service to the initiating entity but is redirecting traffic to another host
        /// under the administrative control of the same service provider.
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.19
        /// </summary>
        SeeOtherHost,


        /// <summary>
        /// The server is being shut down and all active streams are being closed.
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.20
        /// </summary>
        SystemShutdown,


        /// <summary>
        /// The error condition is not one of those defined by the other conditions in this list; this error condition
        /// SHOULD NOT be used except in conjunction with an application-specific condition.
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.21
        /// </summary>
        UndefinedCondition,

        /// <summary>
        /// The initiating entity has encoded the stream in an encoding that is not supported by the server or has
        /// otherwise improperly encoded the stream (e.g., by violating the rules of the UTF-8 encoding).
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.22
        /// </summary>
        UnsupportedEncoding,


        /// <summary>
        /// The receiving entity has advertised a mandatory-to-negotiate stream feature that the initiating entity does
        /// not support, and has offered no other mandatory-to-negotiate feature alongside the unsupported feature.
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.23
        /// </summary>
        UnsupportedFeature,

        /// <summary>
        /// The initiating entity has sent a first-level child of the stream that is not supported by the server, either
        /// because the receiving entity does not understand the namespace or because the receiving entity does not
        /// understand the element name for the applicable namespace (which might be the content namespace declared as
        /// the default namespace).
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.24
        /// </summary>
        UnsupportedStanzaType,


        /// <summary>
        /// The 'version' attribute provided by the initiating entity in the stream header specifies a version of XMPP
        /// that is not supported by the server.
        /// https://tools.ietf.org/html/rfc6120#section-4.9.3.25
        /// </summary>
        UnsupportedVersion,
    }
}