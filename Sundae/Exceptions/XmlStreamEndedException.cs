namespace Sundae
{
    using System;
    using System.Xml;

    public class XmlStreamClosedException : Exception
    {
        public XmlStreamClosedException() : base("The XML stream was closed.") { }
    }
}