using System;

namespace marti_tech_demo.Helper
{
    /// <summary>
    /// Enum tiplerinin dosya uzantılarını attr olarak vermek için kullanılır.
    /// </summary>
    public class ExtensionTypeAttribute : Attribute
    {
        public string ExtensionType { get; set; }
        public ExtensionTypeAttribute(string csv)
        {
            ExtensionType = csv;
        }
    }
}