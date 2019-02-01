namespace Sundae
{
    using System.Threading;
    using System.Xml;
    using System;
    using static XmlEncode;

    public static class XmppCommands
    {
        public static IqStanza SendAuthenticate(
            this XmppConnection xmpp, string user, string password, string resource = null)
        {
            Encode(ref user, ref password, ref resource);

            var id = xmpp.NextId();
            var element = xmpp.SendCustom(id, $@"
                <iq id='{id}' type='set'>
                    <query xmlns='jabber:iq:auth'>
                        <username>{user}</username>
                        <password>{password}</password>
                        <resource>{Random()}</resource>
                    </query>
                </iq>
            ");

            return GetIq(element.Result);
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

            var id = xmpp.NextId();
            var element = xmpp.SendCustom(id, $@"
                <iq id='{id}' type='set' to='{xmpp.Domain}'>
                    <query xmlns='jabber:iq:register'>
                        <username>{user}</username>
                        <password>{password}</password>
                    </query>
                </iq>
            ");

            return GetIq(element.Result);
        }

        public static IqStanza SendRoster(this XmppConnection xmpp)
        {
            var id = xmpp.NextId();
            var element = xmpp.SendCustom(id, $@"
                <iq id='{id}' type='get'>
                    <query xmlns='jabber:iq:roster' />
                </iq>
            ");

            return GetIq(element.Result);
        }

        private static string Random() => new Random().Next().ToString("x");

        private static IqStanza GetIq(XmlElement element) =>
            IqStanza.GetIq(element) ?? throw new UnexpectedXmlException("Expected an \"iq\" element, got:", element);
    }
}