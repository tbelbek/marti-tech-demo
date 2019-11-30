using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace marti_tech_demo.Controllers
{
    [XmlRoot(ElementName = "Zip")]
    public class Zip
    {
        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }
    }

    [XmlRoot(ElementName = "District")]
    public class District
    {
        [XmlElement(ElementName = "Zip")]
        public List<Zip> Zip { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

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

    [XmlRoot("AddressInfo"), XmlType("AddressInfo"), Serializable]
    public class AddressInfo
    {
        [XmlElement(ElementName = "City")]
        public List<City> City { get; set; }

    }

    public class CsvReadModel
    {
        public string CityName { get; set; }
        public int CityCode { get; set; }
        public string DistrictName { get; set; }
        public int ZipCode { get; set; }


    }
    public static class CsvReadModelExtensions
    {
        public static List<City> ConvertToAppDataModel(this List<CsvReadModel> list)
        {
            return list.GroupBy(t => new { t.CityCode, t.CityName }).Select(t => new City
            {
                Name = t.Key.CityName,
                Code = t.Key.CityCode.ToString(),
                District = t.GroupBy(k => k.DistrictName).Select(k => new District()
                {
                    Name = k.Key,
                    Zip = k.Select(o => new Zip
                    {
                        Code = o.ZipCode.ToString(),
                    }).ToList(),
                }).ToList()
            }).ToList();
        }

        public static List<CsvReadModel> ConvertToAppCsvData(this List<City> list)
        {
            var r = from c in list
                    from d in c.District
                    from z in d.Zip
                    select new CsvReadModel { CityCode = Convert.ToInt32(c.Code), CityName = c.Name, DistrictName = d.Name, ZipCode = Convert.ToInt32(z.Code) };

            return r.ToList();
        }


        public static bool AreAllPropertiesNotNullForAllItems(object obj, string query)
        {
            var properties = new ConcurrentBag<PropertyInfo>(obj.GetType().GetProperties());
            var isExist = false;

            Parallel.ForEach(properties,
                (propertyInfo, state) =>
                {
                    if (!propertyInfo.GetValue(obj).ToString().ToLower().Contains(query.ToLower())) return;
                    isExist = true;
                    state.Break();
                });

            return isExist;
        }

        public static class Extensions
        {

        }
    }


}