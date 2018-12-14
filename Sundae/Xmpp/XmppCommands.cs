namespace Sundae
{
    using System.Threading;
    using System.Xml;
    using System;

    public static class XmppCommands
    {
        private static int _id = 0;

        internal static void OpenStream(this XmppStream stream, string domain) =>
            stream.Write($"<stream:stream to='{domain}' xmlns='jabber:client' " +
                "xmlns:stream='http://etherx.jabber.org/streams' version='1.0'>");

        internal static void CloseStream(this XmppStream stream) => stream.Write("</stream>");

        public static XmlElement Authenticate(
            this XmppConnection xmpp, string user, string password, string resource = null)
        {
            var id = Id();

            return xmpp.SendCustom(id, $@"
                <iq id='{id}' type='set'>
                    <query xmlns='jabber:iq:auth'>
                        <username>{user}</username>
                        <password>{password}</password>
                        <resource>{Random()}</resource>
                    </query>
                </iq>
            ");
        }

        public static void Message(this XmppConnection xmpp, string message, string jid) =>
            xmpp.SendCustom($@"
                <message id='{Id()}' type='chat' to='{jid}'>
                    <body>{message}</body>
                </message>
            ");

        public static void Presence(this XmppConnection xmpp) =>
            xmpp.SendCustom($@"
                <presence />
            ");

        public static XmlElement Register(this XmppConnection xmpp, string user, string password)
        {
            var id = Id();

            return xmpp.SendCustom(id, $@"
                <iq id='{id}' type='set' to='{xmpp.Domain}'>
                    <query xmlns='jabber:iq:register'>
                        <username>{user}</username>
                        <password>{password}</password>
                    </query>
                </iq>
            ");
        }

        private static string Id() => Interlocked.Increment(ref _id).ToString();

        private static string Random() => new Random().Next().ToString("x");
    }
}