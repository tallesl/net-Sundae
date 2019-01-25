namespace Sundae
{
    using System.Security;
    using System.Xml;

    internal static class XmlEncode
    {
        internal static void Encode(ref string s1) => s1 = SecurityElement.Escape(s1);

        internal static void Encode(ref string s1, ref string s2)
        {
            Encode(ref s1);
            Encode(ref s2);
        }

        internal static void Encode(ref string s1, ref string s2, ref string s3)
        {
            Encode(ref s1);
            Encode(ref s2);
            Encode(ref s3);
        }

        internal static void Encode(ref string s1, ref string s2, ref string s3, ref string s4)
        {
            Encode(ref s1);
            Encode(ref s2);
            Encode(ref s3);
            Encode(ref s4);
        }

        internal static void Encode(ref string s1, ref string s2, ref string s3, ref string s4, ref string s5)
        {
            Encode(ref s1);
            Encode(ref s2);
            Encode(ref s3);
            Encode(ref s4);
            Encode(ref s5);
        }
    }
}