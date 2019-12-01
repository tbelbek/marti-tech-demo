using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace marti_tech_demo.Models
{
    /// <summary>
    /// XML dosyası ana Node'u, serialize-deserialize için kullanılır.
    /// </summary>
    [XmlRoot("AddressInfo"), XmlType("AddressInfo"), Serializable]
    public class AddressInfo
    {
        [XmlElement(ElementName = "City")]
        public List<City> City { get; set; }

    }
}