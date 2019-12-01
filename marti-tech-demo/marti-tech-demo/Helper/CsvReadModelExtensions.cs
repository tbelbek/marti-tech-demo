using marti_tech_demo.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace marti_tech_demo.Helper
{
    /// <summary>
    /// Verileri birbiri arasında çevirmek için gereken yardımcı metodlar
    /// </summary>
    public static class CsvReadModelExtensions
    {
        /// <summary>
        /// CSV upload modelini uygulama veri modeline çevirir.
        /// </summary>
        /// <param name="list">Upload edilip parse edilmiş csv verisi</param>
        /// <returns>Uygulama veri listesi</returns>
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

        /// <summary>
        /// Uygulama verisini CSV data modeline dönüştürür. 
        /// </summary>
        /// <param name="list">Uygulama veri modeli</param>
        /// <returns>CSV data model listesi döndürür.</returns>
        public static List<CsvReadModel> ConvertToAppCsvData(this List<City> list)
        {
            var r = from c in list
                    from d in c.District
                    from z in d.Zip
                    select new CsvReadModel { CityCode = Convert.ToInt32(c.Code), CityName = c.Name, DistrictName = d.Name, ZipCode = Convert.ToInt32(z.Code) };

            return r.ToList();
        }

        /// <summary>
        /// Tüm veri içerisinde arama algoritması
        /// </summary>
        /// <param name="obj">İçinde değer aranacak obje</param>
        /// <param name="query">Arama sorgusu</param>
        /// <returns>Eğer aranan değer nesne içinde bir property değerinde var ise true döndürür.s</returns>
        public static bool SearchAllPropertyValues(object obj, string query)
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

        /// <summary>
        /// CSV dosya verisini oluşturur.
        /// </summary>
        /// <param name="list">CSV data model listesi</param>
        /// <param name="fileName">Dosya adı</param>
        /// <returns>Filestream döndürür</returns>
        public static FileStream GetCsvStream(this List<CsvReadModel> list, string fileName)
        {
            string name = fileName;

            FileInfo info = new FileInfo(name);

            using (StreamWriter writer = info.CreateText())
            {
                CreateCsvString(list, writer);
            }

            return info.OpenRead();

        }

        public static void CreateCsvString(List<CsvReadModel> list, StreamWriter writer)
        {
            writer.WriteLine(string.Join(",",
                list.FirstOrDefault().GetType().GetProperties().Select(t => t.Name).OrderBy(t => t)));
            foreach (var item in list)
            {
                writer.WriteLine($"{item.CityName},{item.CityCode},{item.DistrictName},{item.ZipCode}");
            }
        }

        /// <summary>
        /// XML Dosya verisi döndürür.
        /// </summary>
        /// <param name="list">CSV data model verisi</param>
        /// <param name="fileName">Dosya adı</param>
        /// <returns>Filestream döndürür</returns>
        public static FileStream GetXmlStream(this List<CsvReadModel> list, string fileName)
        {
            string name = fileName;

            var xmlData = new AddressInfo() { City = list.ConvertToAppDataModel() };

            XmlSerializer serializer = new XmlSerializer(xmlData.GetType());

            FileInfo info = new FileInfo(name);

            using (StreamWriter writer = info.CreateText())
            {
                serializer.Serialize(writer, xmlData);
            }
            
            return info.OpenRead();

        }
    }
}