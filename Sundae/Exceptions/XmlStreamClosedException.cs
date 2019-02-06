namespace Sundae
{
    using System;
    using System.Xml;

    /// <summary>
    /// The XMPP server closed the XML stream.
    /// </summary>
    public class XmlStreamClosedException : Exception
    {
        internal XmlStreamClosedException(XmlElement lastElement): base(
            lastElement.Name == "stream:error" ?
            "The XML stream was closed due to an error:" +
            $"{Environment.NewLine}{Environment.NewLine}{lastElement.OuterXml}" :
            "The XML stream was closed.") { }
    }
}