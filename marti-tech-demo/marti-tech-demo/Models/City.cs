using System.Collections.Generic;
using System.Xml.Serialization;

namespace marti_tech_demo.Models
{
    /// <summary>
    /// XML parse için kullanılır. Aynı zamanda uygulama ana veri modeli olarak da kullanılır.
    /// </summary>
    [XmlRoot(ElementName = "City")]
    public class City
    {
        [XmlElement(ElementName = "District")]
        public List<District> District { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }
    }
}