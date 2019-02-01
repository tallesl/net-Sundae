using Sundae;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using System;
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

        using (var xmpp = new XmppConnection(host, port, domain))
        {
            var pending = new BlockingCollection<XmlElement>();
            var source = new CancellationTokenSource();
            var commands = new Dictionary<string, Action>
            {
                { "authenticate", () => xmpp.SendAuthenticate(user, password, resource) },
                { "connect", () => xmpp.Connect() },
                { "disconnect", () => source.Cancel() },
                { "register", () => xmpp.SendRegister(user, password) },
                { "message", () => xmpp.SendMessage("Test message.", $"{recipient}@{domain}") },
                { "presence", () => xmpp.SendPresence() },
                { "roster", () => xmpp.SendRoster() }
            };

            xmpp.OnStreamError += (_, e) => Console.Error.WriteLine($"Stream error: {e.Text ?? e.DefinedCondition}");
            xmpp.OnElement += (_, e) => pending.Add(e);
            xmpp.OnException += (_, e) =>
            {
                Console.Error.WriteLine(e);
                Environment.Exit(1);
            };

            Console.WriteLine($"Available commands: {string.Join(", ", commands.Keys)}.");

            while (!source.Token.IsCancellationRequested)
            {
                Action action;

                var currentTimeout = 300;
                var command = Ask("command").Trim().ToLower();

                if (command == string.Empty)
                    currentTimeout = timeout;

                else if (commands.TryGetValue(command, out action))
                    action();

                else
                    Console.WriteLine($"Invalid command.{Environment.NewLine}");

                PrintReceived(pending.TakeAll(currentTimeout));
            }
        }
    }
}