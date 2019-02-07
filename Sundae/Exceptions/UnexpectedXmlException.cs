namespace Sundae
{
    using System;
    using System.Xml;

    /// <summary>
    /// An unexpected XML element was found.
    /// </summary>
    public class UnexpectedXmlException : Exception
    {
        internal UnexpectedXmlException(string message, XmlElement element):
            base($"{message}{Environment.NewLine}{Environment.NewLine}{element.Beautify()}{Environment.NewLine}") { }
    }
}