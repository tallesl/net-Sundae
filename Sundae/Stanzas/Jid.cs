namespace Sundae
{
    using System.Xml;

    /// <summary>
    /// Jabber identifier (JID), the network address of an XMPP entity (local@domain/resource).
    ///  https://tools.ietf.org/html/rfc7622
    /// </summary>
    public class Jid
    {
        internal Jid(string jid)
        {
            var at = jid.IndexOf('@');
            var hasLocal = at != -1;

            var slash = hasLocal ? jid.IndexOf('/', at) : -1;
            var hasResource = slash != -1;

            Local = hasLocal ? jid.Substring(0, at) : null;
            Resource = hasResource ? jid.Substring(slash + 1, jid.Length - slash - 1) : null;

            if (hasLocal && hasResource)
                Domain = jid.Substring(at + 1, jid.Length - Local.Length - Resource.Length - 2);

            else if (hasLocal && !hasResource)
                Domain = jid.Substring(at + 1, jid.Length - Local.Length - 1);

            else
                Domain = jid;
        }

        /// <summary>
        /// Uniquely identifies the entity requesting and using network access provided by a server (a local account).
        /// https://tools.ietf.org/html/rfc7622#section-3.3
        /// </summary>
        public string Local { get; private set; }

        /// <summary>
        /// Identifies the "home" server to which clients connect for XML routing and data management functionality.
        /// https://tools.ietf.org/html/rfc7622#section-3.2
        /// </summary>
        public string Domain { get; private set; }

        /// <summary>
        /// Uniquely identifies a specific connection (a device or location).
        /// https://tools.ietf.org/html/rfc7622#section-3.4
        /// </summary>
        public string Resource { get; private set; }

        public override string ToString()
        {
            var hasLocal = Local != null;
            var hasResource = Resource != null;

            if (hasLocal && hasResource)
                return $"{Local}@{Domain}/{Resource}";

            else if (hasLocal && !hasResource)
                return $"{Local}@{Domain}";

            else
                return Domain;
        }

        public static implicit operator string(Jid jid) => jid.ToString();

        internal static Jid GetJid(XmlElement element, string attribute)
        {
            var value = element.GetAttribute(attribute);
            return string.IsNullOrEmpty(value) ? null : new Jid(value);
        }

        internal static Jid GetJidOrThrow(XmlElement element, string attribute)
        {
            var value = element.GetAttributeOrThrow(attribute);
            return string.IsNullOrEmpty(value) ? null : new Jid(value);
        }
    }
}