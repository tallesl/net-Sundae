namespace Sundae
{
    using System.Threading;
    using System.Xml;
    using System;
    using static IqStanza;
    using static XmlEncode;

    /// <summary>
    /// Commonly used stanzas to send.
    /// </summary>
    public static class XmppCommands
    {
        /// <summary>
        /// Sends a set IQ with username, password and resource authenticating the user.
        /// https://xmpp.org/extensions/xep-0078.html
        /// </summary>
        /// <param name="xmpp">XMPP connection to use</param>
        /// <param name="username">User's username</param>
        /// <param name="password">User's password</param>
        /// <param name="resource">
        /// Resource identifying the connection (null to use a random string)
        /// </param>
        /// <param name="timeout">Timeout of the request in milliseconds (null to wait indefinitely)</param>
        /// <returns>The resulted IQ stanza</returns>
        public static IqStanza SendAuthenticate(
            this XmppConnection xmpp, string username, string password, string resource = null, int? timeout = null)
        {
            resource = resource ?? Random();
            Encode(ref username, ref password, ref resource);

            return xmpp.SendIq($@"
                <iq type='set'>
                    <query xmlns='jabber:iq:auth'>
                        <username>{username}</username>
                        <password>{password}</password>
                        <resource>{resource}</resource>
                    </query>
                </iq>",
                timeout
            );
        }

        /// <summary>
        /// Sends an IQ stanza to the server.
        /// Setting up the id attribute is optional (a random string is used if none is provided).
        /// </summary>
        /// <param name="xmpp">XMPP connection to use</param>
        /// <param name="xml">XML of the IQ stanza to send</param>
        /// <param name="timeout">Timeout of the request in milliseconds (null to wait indefinitely)</param>
        /// <returns>The resulted IQ stanza</returns>
        public static IqStanza SendIq(this XmppConnection xmpp, string xml, int? timeout = null) =>
            xmpp.SendIq(xml.ToXmlElement(), timeout);

        /// <summary>
        /// Sends an IQ stanza to the server.
        /// Setting up the id attribute is optional (a random string is used if none is provided).
        /// </summary>
        /// <param name="xmpp">XMPP connection to use</param>
        /// <param name="xml">XML of the IQ stanza to send</param>
        /// <param name="timeout">Timeout of the request in milliseconds (null to wait indefinitely)</param>
        /// <returns>The resulted IQ stanza</returns>
        public static IqStanza SendIq(this XmppConnection xmpp, XmlElement xml, int? timeout = null)
        {
            if (xml.Name != "iq")
                throw new UnexpectedXmlException("Expected \"iq\" element:", xml);

            var result = xmpp.SendCustomWithResult(xml, timeout).Result;
            return GetIq(result) ?? throw new UnexpectedXmlException("Expected an \"iq\" element, got:", result);
        }

        /// <summary>
        /// Sends a message stanza to the server.
        /// </summary>
        /// <param name="xmpp">XMPP connection to use</param>
        /// <param name="to">JID of the intended recipient</param>
        /// <param name="body">Human-readable textual contents of the message</param>
        public static void SendMessage(this XmppConnection xmpp, string to, string body)
        {
            Encode(ref to, ref body);

            xmpp.SendCustom($@"
                <message type='chat' to='{to}'>
                    <body>{body}</body>
                </message>
            ");
        }

        /// <summary>
        /// Sends an empty presence stanza.
        /// </summary>
        /// <param name="xmpp">XMPP connection to use</param>
        public static void SendPresence(this XmppConnection xmpp) => xmpp.SendCustom("<presence />");

        /// <summary>
        /// Sends a set IQ with username and password registering the user.
        /// https://xmpp.org/extensions/xep-0077.html
        /// </summary>
        /// <param name="xmpp">XMPP connection to use</param>
        /// <param name="username">User's username</param>
        /// <param name="password">User's password</param>
        /// <param name="timeout">Timeout of the request in milliseconds (null to wait indefinitely)</param>
        /// <returns>The resulted IQ stanza</returns>
        public static IqStanza SendRegister(
            this XmppConnection xmpp, string username, string password, int? timeout = null)
        {
            Encode(ref username, ref password);

            return xmpp.SendIq($@"
                <iq type='set' to='{xmpp.Domain}'>
                    <query xmlns='jabber:iq:register'>
                        <username>{username}</username>
                        <password>{password}</password>
                    </query>
                </iq>",
                timeout
            );
        }

        /// <summary>
        /// Sends a get IQ requesting user's roster.
        /// https://tools.ietf.org/html/rfc6121#section-2.1.3
        /// </summary>
        /// <param name="xmpp">XMPP connection to use</param>
        /// <param name="timeout">Timeout of the request in milliseconds (null to wait indefinitely)</param>
        /// <returns>The resulted IQ stanza</returns>
        public static IqStanza SendRoster(this XmppConnection xmpp, int? timeout = null) =>
            xmpp.SendIq($@"
                <iq type='get'>
                    <query xmlns='jabber:iq:roster' />
                </iq>",
                timeout
            );

        private static string Random() => new Random().Next().ToString("x");
    }
}