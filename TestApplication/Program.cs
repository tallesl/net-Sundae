using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Xml;
using System;
using Sundae;
using static Cli;

public static class Program
{
    public static void Main(string[] args)
    {
        var host = args.Get("host");
        var port = int.Parse(args.Get("port"));

        var user = args.Get("user");
        var domain = args.Get("domain");
        var password = args.Get("password");
        var recipient = args.Get("recipient");

        var resource = args.Get("resource");
        var timeout = int.Parse(args.Get("timeout"));

        var pending = new BlockingCollection<XmlElement>();

        using (var xmpp = new XmppConnection(host, port, domain))
        {
            xmpp.OnElement += (_, e) => pending.Add(e);
            xmpp.OnException += (_, e) =>
            {
                Console.Error.WriteLine(e);
                Environment.Exit(1);
            };

            for (;;)
            {
                var wait = false;

                switch (Ask("command").Trim())
                {
                    case "":
                        wait = true;
                        break;

                    case "authenticate":
                        xmpp.SendCustom($@"
                            <iq id='{Random()}' type='set'>
                                <query xmlns='jabber:iq:auth'>
                                    <username>{user}</username>
                                    <resource>{resource}</resource>
                                    <password>{password}</password>
                                </query>
                            </iq>
                        ");
                        break;

                    case "connect":
                        xmpp.Connect();
                        break;

                    case "disconnect":
                        return;

                    case "register":
                        xmpp.SendCustom($@"
                            <iq id='{Random()}' type='set' to='{domain}'>
                                <query xmlns='jabber:iq:register'>
                                    <username>{user}</username>
                                    <password>{password}</password>
                                </query>
                            </iq> 
                        ");
                        break;

                    case "message":
                        xmpp.SendCustom($@"
                            <message id='{Random()}' type='chat' to='{recipient}@{domain}'>
                                <body>Test message.</body>
                            </message>
                        ");
                        break;

                    case "presence":
                        xmpp.SendCustom("<presence />");
                        break;

                    default:
                        Console.WriteLine("Invalid command.");
                        Console.WriteLine();
                        break;
                }

                var received = wait ? pending.TakeAll(timeout) : pending.TakeAll();

                PrintReceived(received);
            }
        }
    }
}