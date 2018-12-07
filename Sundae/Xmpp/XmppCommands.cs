namespace Sundae
{
    using System;
    using System.Threading;

    public static class XmppCommands
    {
        private static int _id = 0;

        private static int Id => Interlocked.Increment(ref _id);

        private static string Random => new Random().Next().ToString("x");

        internal static void OpenStream(this XmppStream stream, string domain) =>
            stream.Write($"<stream:stream to='{domain}' xmlns='jabber:client' " +
                "xmlns:stream='http://etherx.jabber.org/streams' version='1.0'>");

        internal static void CloseStream(this XmppStream stream) => stream.Write("</stream>");

        public static void Authenticate(
            this XmppConnection xmpp, string user, string password, string resource = null) =>
            xmpp.SendCustom($@"
                <iq id='{Random}' type='set'>
                    <query xmlns='jabber:iq:auth'>
                        <username>{user}</username>
                        <password>{password}</password>
                        <resource>{Random}</resource>
                    </query>
                </iq>
            ");

        public static void Message(this XmppConnection xmpp, string message, string jid) =>
            xmpp.SendCustom($@"
                <message id='{Random}' type='chat' to='{jid}'>
                    <body>{message}</body>
                </message>
            ");

        public static void Presence(this XmppConnection xmpp) =>
            xmpp.SendCustom($@"
                <presence />
            ");

        public static void Register(this XmppConnection xmpp, string user, string password) =>
            xmpp.SendCustom($@"
                <iq id='{Random}' type='set' to='{xmpp.Domain}'>
                    <query xmlns='jabber:iq:register'>
                        <username>{user}</username>
                        <password>{password}</password>
                    </query>
                </iq>
            ");
    }
}