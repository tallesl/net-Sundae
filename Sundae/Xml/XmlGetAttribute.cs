namespace Sundae
{
    using System.Xml;

    internal static class XmlGetAttribute
    {
        internal static int? GetIntegerAttribute(this XmlElement element, string name)
        {
            var value = element.GetAttribute(name);

            if (string.IsNullOrEmpty(value))
                return null;

            int i;

            if (!int.TryParse(value, out i))
                throw new UnexpectedXmlException($"\"{name}\" should be an integer:", element);

            return i;
        }

        internal static string GetAttributeOrThrow(this XmlElement element, string name)
        {
            var value = element.GetAttribute(name);

            if (string.IsNullOrEmpty(value))
                throw new UnexpectedXmlException($"Missing \"{name}\" attribute:", element);

            return value;
        }
    }
}