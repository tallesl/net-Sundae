namespace Sundae
{
    using System.Xml;

    internal static class XmlGetAttribute
    {
        internal static string GetAttributeOrThrow(this XmlElement element, string name)
        {
            var attribute = element.GetAttribute(name);

            if (string.IsNullOrEmpty(attribute))
                throw new UnexpectedXmlException($"Missing \"{name}\" attribute:", element);

            return attribute;
        }
    }
}