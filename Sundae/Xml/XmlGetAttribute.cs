namespace Sundae
{
    using System.Xml;

    internal static class XmlGetAttribute
    {
        internal static string GetAttributeOrThrow(this XmlElement element, string name) =>
            element.GetAttribute(name) ?? throw new UnexpectedXmlException($"Missing \"{name}\" attribute:", element);
    }
}