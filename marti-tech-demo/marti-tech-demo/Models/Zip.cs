using System.Xml.Serialization;

namespace marti_tech_demo.Models
{
    /// <summary>
    /// Xml zip parse verisi data modeli.
    /// </summary>
    [XmlRoot(ElementName = "Zip")]
    public class Zip
    {
        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }
    }
}