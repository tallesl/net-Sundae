namespace Sundae
{
    using System;

    public static class Xml
    {
        public static void Authenticate(
            this XmppConnection xmpp, string user, string password, string resource = null) =>
            xmpp.SendCustom($@"
                <iq id='{Random()}' type='set'>
                    <query xmlns='jabber:iq:auth'>
                        <username>{user}</username>
                        <password>{password}</password>
                        <resource>{resource ?? Random("resource")}</resource>
                    </query>
                </iq>
            ");

        public static void Message(this XmppConnection xmpp, string message, string recipientJid) =>
            xmpp.SendCustom($@"
                <message id='{Random()}' type='chat' to='{recipientJid}'>
                    <body>{message}</body>
                </message>
            ");

        public static void Presence(this XmppConnection xmpp) =>
            xmpp.SendCustom($@"
                <presence />
            ");

        public static void Register(this XmppConnection xmpp, string user, string password) =>
            xmpp.SendCustom($@"
                <iq id='{Random()}' type='set' to='{xmpp.Domain}'>
                    <query xmlns='jabber:iq:register'>
                        <username>{user}</username>
                        <password>{password}</password>
                    </query>
                </iq>
            ");

        private static string Random(string text = null)
        {
            var random = new Random().Next().ToString();
            return text == null ? random : $"{text}-{random}";
        }
    }
}