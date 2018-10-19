using System;

namespace UriPathScanf.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UrlMetaAttribute : Attribute
    {
        public string BindName { get; }
        public UrlMetaAttribute(string bindName)
        {
            BindName = bindName;
        }
    }
}
