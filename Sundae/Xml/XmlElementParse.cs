namespace Sundae
{
    using System.Xml;

    internal static class XmlElementParse
    {
        internal static XmlElement ToXmlElement(this string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc.DocumentElement;
        }
    }
}