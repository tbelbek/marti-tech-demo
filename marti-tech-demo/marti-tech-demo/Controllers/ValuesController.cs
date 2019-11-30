using marti_tech_demo.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace marti_tech_demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IServiceProvider _serviceProvider;
        private static IEnumerable<City> ModelList { get; set; }

        public ValuesController(ITokenProvider tokenProvider, IServiceProvider serviceProvider)
        {
            _tokenProvider = tokenProvider;
            _serviceProvider = serviceProvider;
        }

        // GET api/values
        [HttpGet]
        [Route("/api/get-token")]
        public IActionResult GetToken()
        {
            return this.Ok(_tokenProvider.TokenGenerator());
        }

        [HttpPost]
        [Route("/api/upload-data-file")]
        public IActionResult UploadFile(IFormFile file, FileType extension)
        {
            if (Path.GetExtension(file.FileName).ToUpper().Replace(".", string.Empty) != extension.GetAttribute<ExtensionTypeAttribute>().ExtensionType.ToUpper())
                return BadRequest("Yüklenen dosya tipi seçilenden farklı.");


            var parsedList = extension == FileType.CSV
                ? (_serviceProvider.GetService(typeof(CsvParser)) as CsvParser)?.ParseFile(file)
                .ToList()
                : (_serviceProvider.GetService(typeof(XmlParser)) as XmlParser)?.ParseFile(file)
                .ToList();

            ModelListGenerator(parsedList);

            return Ok("File Uploaded");
        }

        [HttpPost]
        [Route("/api/query-and-sort")]
        public IActionResult QueryAndSort(string query, string sort)
        {
            var resultList = ModelList.ToList().ConvertToAppCsvData();

            if (!string.IsNullOrEmpty(query))
            {
                //CsvReadModelExtensions.AreAllPropertiesNotNullForAllItems(resultList.First(), query);
                resultList = resultList.Where(t => CsvReadModelExtensions.AreAllPropertiesNotNullForAllItems(t, query)).ToList();
            }

            if (!string.IsNullOrEmpty(sort))
            {
                System.Reflection.PropertyInfo prop = typeof(CsvReadModel).GetProperty(sort);

                resultList = resultList.OrderBy(x => prop.GetValue(x, null)).ToList();
            }

            return Ok(resultList.ConvertToAppDataModel());
        }

        private void ModelListGenerator(IEnumerable<City> list)
        {
            ModelList = new ConcurrentBag<City>(list);
        }
    }

    public enum FileType
    {
        [ExtensionType("csv")]
        CSV,
        [ExtensionType("xml")]
        XML
    }


    public class ExtensionTypeAttribute : Attribute
    {
        public string ExtensionType { get; set; }
        public ExtensionTypeAttribute(string csv)
        {
            ExtensionType = csv;
        }
    }

    public static class EnumHelper
    {
        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        /// <example>string desc = myEnumVariable.GetAttributeOfType<DescriptionAttribute>().Description;</example>
        public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            var enumType = value.GetType();
            var name = Enum.GetName(enumType, value);
            return enumType.GetField(name).GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
        }
    }

    public interface IFileParser
    {
        IEnumerable<City> ParseFile(IFormFile file);
    }

    public class XmlParser : IFileParser
    {
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
