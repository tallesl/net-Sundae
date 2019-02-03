namespace Sundae
{
    using System;
    using System.Xml;

    /// <summary>
    /// The XMPP server closed the XML stream.
    /// </summary>
    public class XmlStreamClosedException : Exception
    {
        internal XmlStreamClosedException() : base("The XML stream was closed.") { }
    }
}