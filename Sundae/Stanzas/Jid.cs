namespace Sundae
{
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

        public string Local { get; internal set; }

        public string Domain { get; internal set; }

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