using System.Collections.Generic;
using System.Xml.Serialization;

namespace marti_tech_demo.Models
{
    /// <summary>
    /// XML district parse verisi.
    /// </summary>
    [XmlRoot(ElementName = "District")]
    public class District
    {
        [XmlElement(ElementName = "Zip")]
        public List<Zip> Zip { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }
}