namespace Sundae
{
    using System.Linq;
    using System.Xml;

    internal static class XmlDefinedCondition
    {
        internal static string GetDefinedCondition(this XmlElement errorElement)
        {
            var definedConditions = errorElement.Children().Where(e => e.Name != "text");

            if (!definedConditions.Any())
                throw new UnexpectedXmlException("No defined condition element found:", errorElement);

            if (definedConditions.Count() > 1)
                throw new UnexpectedXmlException("Multiple defined conditions found:", errorElement);

            return definedConditions.Single().Name;
        }
    }
}