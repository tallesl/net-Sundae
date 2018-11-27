namespace Sundae
{
    using System;
    using System.Xml;

    public class UnexpectedXmlException : Exception
    {
        public UnexpectedXmlException(string message, XmlElement element):
            base($"{message}{Environment.NewLine}{Environment.NewLine}{element.OuterXml}") { }
    }
}