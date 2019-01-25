namespace Sundae
{
    using System.Threading;
    using System.Xml;
    using System;
    using static XmlEncode;

    public static class XmppCommands
    {
        private static int _id = 0;

        internal static void SendOpenStream(this XmppStream stream, string domain)
        {
            Encode(ref domain);
            stream.Write($"<stream:stream to='{domain}' xmlns='jabber:client' " +
                "xmlns:stream='http://etherx.jabber.org/streams' version='1.0'>");
        }

        internal static void SendCloseStream(this XmppStream stream) => stream.Write("</stream>");

        public static IqStanza SendAuthenticate(
            this XmppConnection xmpp, string user, string password, string resource = null)
        {
            Encode(ref user, ref password, ref resource);

            var id = Id();
            var element = xmpp.SendCustom(id, $@"
                <iq id='{id}' type='set'>
                    <query xmlns='jabber:iq:auth'>
                        <username>{user}</username>
                        <password>{password}</password>
                        <resource>{Random()}</resource>
                    </query>
                </iq>
            ");

            return GetIq(element);
        }

        public static void SendMessage(this XmppConnection xmpp, string message, string jid)
        {
            Encode(ref message, ref jid, ref message);
            xmpp.SendCustom($@"
                <message id='{Id()}' type='chat' to='{jid}'>
                    <body>{message}</body>
                </message>
            ");
        }

        public static void SendPresence(this XmppConnection xmpp) => xmpp.SendCustom("<presence />");

        public static IqStanza SendRegister(this XmppConnection xmpp, string user, string password)
        {
            Encode(ref user, ref password);

            var id = Id();
            var element = xmpp.SendCustom(id, $@"
                <iq id='{id}' type='set' to='{xmpp.Domain}'>
                    <query xmlns='jabber:iq:register'>
                        <username>{user}</username>
                        <password>{password}</password>
                    </query>
                </iq>
            ");

            return GetIq(element);
        }

        public static IqStanza SendRoster(this XmppConnection xmpp)
        {
            var id = Id();
            var element = xmpp.SendCustom(id, $@"
                <iq id='{id}' type='get'>
                    <query xmlns='jabber:iq:roster' />
                </iq>
            ");

            return GetIq(element);
        }

        private static string Id() => Interlocked.Increment(ref _id).ToString();

        private static string Random() => new Random().Next().ToString("x");

        private static IqStanza GetIq(XmlElement element) =>
            IqStanza.GetIq(element) ?? throw new UnexpectedXmlException("Expected an \"iq\" element, got:", element);
    }
}