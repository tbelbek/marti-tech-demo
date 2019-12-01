using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using marti_tech_demo.Models;
using Microsoft.AspNetCore.Http;

namespace marti_tech_demo.Helper
{
    /// <summary>
    /// XML dosyasını parse işlemleri.
    /// </summary>
    public class XmlParser : IFileParser
    {
        /// <summary>
        /// XML dosyasını uygulama veri modeline dönüştürür.
        /// </summary>
        public IEnumerable<City> ParseFile(IFormFile file)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AddressInfo));
            using (Stream stream = file.OpenReadStream())
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    return ((AddressInfo)serializer.Deserialize(reader)).City;
                }
            }
        }
    }
}