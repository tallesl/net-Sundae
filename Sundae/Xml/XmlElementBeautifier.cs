namespace Sundae
{
    using System.Xml;
    using System.Xml.Linq;

    internal static class XmlElementBeautifier
    {
        internal static string Beautify(this XmlElement element) => XElement.Parse(element.OuterXml).ToString();
    }
}