namespace Sundae
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;

    internal static class XmlChildNodes
    {
        internal static XmlElement SingleChild(this XmlElement element, string name) =>
            element.SingleChildOrDefault(name) ??
                throw new UnexpectedXmlException($"No \"{name}\" element found:", element);

        internal static XmlElement SingleChildOrDefault(this XmlElement element, string name)
        {
            var children = element.Children().Where(e => e.Name == name);

            if (children.Count() > 1)
                throw new UnexpectedXmlException($"Multiple \"{name}\" found:", element);

            return children.SingleOrDefault();
        }
        
        internal static IEnumerable<XmlElement> Children(this XmlElement element) =>
            element.ChildNodes.Cast<XmlElement>();
    }
}