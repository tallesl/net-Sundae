namespace Sundae
{
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
        public string Local { get; internal set; }

        /// <summary>
        /// Identifies the "home" server to which clients connect for XML routing and data management functionality.
        /// https://tools.ietf.org/html/rfc7622#section-3.2
        /// </summary>
        public string Domain { get; internal set; }

        /// <summary>
        /// Uniquely identifies a specific connection (a device or location).
        /// https://tools.ietf.org/html/rfc7622#section-3.4
        /// </summary>
        public string Resource { get; internal set; }

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
    }
}