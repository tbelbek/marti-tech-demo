using System;
using System.Collections.Generic;
using System.IO;
using marti_tech_demo.Models;
using Microsoft.AspNetCore.Http;

namespace marti_tech_demo.Helper
{
    /// <summary>
    /// Upload edilen dosyayı uygulama data tipine çevirir.
    /// </summary>
    public class CsvParser : IFileParser
    {
        public IEnumerable<City> ParseFile(IFormFile file)
        {
            List<CsvReadModel> list = new List<CsvReadModel>();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                reader.ReadLine();
                while (reader.Peek() >= 0)
                {
                    var lineData = reader.ReadLine()?.Split(",");
                    list.Add(new CsvReadModel()
                    {
                        CityName = lineData[0],
                        CityCode = Convert.ToInt32(lineData[1]),
                        DistrictName = lineData[2],
                        ZipCode = Convert.ToInt32(lineData[3])
                    });
                }
            }

            return list.ConvertToAppDataModel();
        }
    }
}