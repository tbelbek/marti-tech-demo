using marti_tech_demo.Helper;
using marti_tech_demo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;

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
        /// <summary>
        /// Diğer api'ların kullanılabilmesi için tek seferlik token alınmalıdır. Bir token süresi 360 dk'dır ve appsettings.json'dan değiştirilebilir.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/swagger/get-token")]
        public IActionResult GetToken()
        {
            return this.Ok(_tokenProvider.TokenGenerator());
        }

        /// <summary>
        /// Uygulama veri dosyalarını yüklemeye yarar. XML ve CSV destekler.
        /// </summary>
        /// <param name="file">Dosya</param>
        /// <param name="extension">Dosya tipi</param>
        /// <returns>String sonuç döndürür</returns>
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

            ModelList = new ConcurrentBag<City>(parsedList);

            return Ok("File Uploaded");
        }

        /// <summary>
        /// Uygulama içerisine atılan veriyi filtreler ve sıralar. Filtreleme opsiyonu verilen string bilgisini uygulamadaki verilerin tamamının verilerinde arar. Filtreleme için 4 farklı tip string key verilerek yapılabilir. Dosya tipi seçilmezse JSON string döndürür. Seçilen dosya tipine göre fileresult döndürür.
        /// </summary>
        /// <param name="query">String arama sorgusu</param>
        /// <param name="sort">String sort key(CityName,CityCode,District,Zip), value (true asc, false desc)</param>
        /// <param name="type">File result tipi, boş bırakılırsa JsonResult döndürür.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/query-and-sort")]
        public IActionResult QueryAndSort(string query, Dictionary<string, bool> sort, FileType? type)
        {
            var resultList = ModelList.ToList().ConvertToAppCsvData();

            if (!string.IsNullOrEmpty(query))
            {
                //CsvReadModelExtensions.AreAllPropertiesNotNullForAllItems(resultList.First(), query);
                resultList = resultList.Where(t => CsvReadModelExtensions.SearchAllPropertyValues(t, query)).ToList();
            }

            if (sort != null && sort.ToList().Any())
            {
                var queryParams = sort.Select(t => t.Value ? $"{t.Key} ascending" : $"{t.Key} descending");

                resultList = resultList.AsQueryable().OrderBy(string.Join(",", queryParams)).ToList();
            }


            if (type != null)
            {
                switch (type)
                {
                    case FileType.CSV:
                        return File(resultList.GetCsvStream($"csv-result-{Guid.NewGuid().ToString("n").ToLower()}.csv"),
                            "text/csv");
                    case FileType.XML:
                        return File(resultList.GetXmlStream($"xml-result-{Guid.NewGuid().ToString("n").ToLower()}.csv"),
                            "text/xml");
                }
            }

            return Ok(resultList.ConvertToAppDataModel());
        }


    }
}
