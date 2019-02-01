namespace Sundae
{
    using System.Xml;

    internal static class XmlElementParse
    {
        internal static XmlElement ToXmlElement(this string data)
        {
            var xml = new XmlDocument(); 
            xml.LoadXml(data); 
            return xml.DocumentElement;
        }
    }
}