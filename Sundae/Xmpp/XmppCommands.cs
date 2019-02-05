namespace Sundae
{
    using System.Threading;
    using System.Xml;
    using System;
    using static IqStanza;
    using static XmlEncode;

    public static class XmppCommands
    {
        public static IqStanza SendAuthenticate(
            this XmppConnection xmpp, string user, string password, string resource = null)
        {
            resource = resource ?? Random();
            Encode(ref user, ref password, ref resource);

            return xmpp.SendIq($@"
                <iq type='set'>
                    <query xmlns='jabber:iq:auth'>
                        <username>{user}</username>
                        <password>{password}</password>
                        <resource>{resource}</resource>
                    </query>
                </iq>
            ");
        }

        public static IqStanza SendIq(this XmppConnection xmpp, string data, int? millisecondsTimeout = null) =>
            xmpp.SendIq(data.ToXmlElement(), millisecondsTimeout);

        public static IqStanza SendIq(this XmppConnection xmpp, XmlElement data, int? millisecondsTimeout = null)
        {
            if (data.Name != "iq")
                throw new UnexpectedXmlException($"Expected \"iq\" element:", data);

            var result = xmpp.SendCustomWithResult(data, millisecondsTimeout).Result;
            return GetIq(result) ?? throw new UnexpectedXmlException("Expected an \"iq\" element, got:", result);
        }

        public static void SendMessage(this XmppConnection xmpp, string message, string jid)
        {
            Encode(ref message, ref jid, ref message);

            xmpp.SendCustom($@"
                <message type='chat' to='{jid}'>
                    <body>{message}</body>
                </message>
            ");
        }

        public static void SendPresence(this XmppConnection xmpp) => xmpp.SendCustom("<presence />");

        public static IqStanza SendRegister(this XmppConnection xmpp, string user, string password)
        {
            Encode(ref user, ref password);

            return xmpp.SendIq($@"
                <iq type='set' to='{xmpp.Domain}'>
                    <query xmlns='jabber:iq:register'>
                        <username>{user}</username>
                        <password>{password}</password>
                    </query>
                </iq>
            ");
        }

        public static IqStanza SendRoster(this XmppConnection xmpp) =>
            xmpp.SendIq($@"
                <iq type='get'>
                    <query xmlns='jabber:iq:roster' />
                </iq>
            ");

        private static string Random() => new Random().Next().ToString("x");
    }
}