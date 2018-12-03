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
                        xmpp.Authenticate(user, password, resource);
                        break;

                    case "connect":
                        xmpp.Connect();
                        break;

                    case "disconnect":
                        return;

                    case "register":
                        xmpp.Register(user, password);
                        break;

                    case "message":
                        xmpp.Message("Test message.", $"{recipient}@{domain}");
                        break;

                    case "presence":
                        xmpp.Presence();
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