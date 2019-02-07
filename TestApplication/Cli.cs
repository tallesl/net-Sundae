using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System;

public static class Cli
{
    public static void Print(object o)
    {
        Clear();
        Console.WriteLine(o);
        Console.WriteLine();
    }

    public static void PrintReceived(IEnumerable<XmlElement> elements)
    {
        Print("Received: ");

        // Deferred execution, that's why no Any().
        var nothing = true;

        Console.Write("Waiting...");

        foreach (var e in elements)
        {
            nothing = false;

            Print(e.OuterXml);
            Console.Write("Waiting...");
        }

        Clear();

        if (nothing)
            Print("Nothing.");
    }

    public static string Random(string text = null)
    {
        var random = new Random().Next().ToString();

        return text == null ? random : $"{text}-{random}";
    }
    
    public static string ReplaceRandom(string text)
    {
        var isRandom = text.ToLower().Trim() == "random";

        return isRandom ? Random() : text;
    }

    public static string Ask(string label)
    {
        var capitalized = char.ToUpper(label[0]) + label.Substring(1);
        Console.Write($"{capitalized}: ");

        var read = Console.ReadLine();
        Console.WriteLine();

        return ReplaceRandom(read);
    }

    public static string Get(this string[] args, string option)
    {
        var index = Array.IndexOf(args, $"--{option}");
        var next = index + 1;

        var hasNext = index >= 0 && index < (args.Length - 1);
        var nextIsOption = hasNext && args[next].StartsWith("--");

        return hasNext && !nextIsOption ? ReplaceRandom(args[next]) : Ask(option);
    }

    private static void Clear() => Console.Write("\r               \r");
}