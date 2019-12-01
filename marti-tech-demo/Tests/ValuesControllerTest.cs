using marti_tech_demo.Controllers;
using marti_tech_demo.Helper;
using marti_tech_demo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class ValuesControllerTest
    {
        private Mock<ITokenProvider> _tokenProvider;
        private Mock<IServiceProvider> _serviceProvider;
        private List<CsvReadModel> CsvList;

        [TestInitialize]
        public void SetUp()
        {
            _tokenProvider = new Mock<ITokenProvider>();
            _serviceProvider = new Mock<IServiceProvider>();

            _serviceProvider.Setup(mock => mock.GetService(typeof(CsvParser))).Returns(new CsvParser());
            _serviceProvider.Setup(mock => mock.GetService(typeof(XmlParser))).Returns(new XmlParser());
        }

        [TestMethod]
        public void TokenTest()
        {
            var controller = new ValuesController(_tokenProvider.Object, _serviceProvider.Object);
            var result = controller.GetToken();
            Assert.AreEqual(typeof(OkObjectResult), result.GetType());

        }

        [TestMethod]
        public void UploadXmlTest()
        {
            var controller = new ValuesController(_tokenProvider.Object, _serviceProvider.Object);

            StreamReader readerXml = new StreamReader("SampleData\\sample_data.xml");

            IFormFile fileXml = new FormFile(readerXml.BaseStream, 0, readerXml.BaseStream.Length, "Data", "dummy.xml");

            var resultXML = controller.UploadFile(fileXml, FileType.XML);
            Assert.AreEqual(typeof(OkObjectResult), resultXML.GetType());

        }

        [TestMethod]
        public void UploadErrorTest()
        {
            var controller = new ValuesController(_tokenProvider.Object, _serviceProvider.Object);

            StreamReader readerBad = new StreamReader("SampleData\\sample_data.csv");

            IFormFile fileBad = new FormFile(readerBad.BaseStream, 0, 0, "Data", "dummy.txt");

            var resultBad = controller.UploadFile(fileBad, FileType.CSV);
            Assert.AreEqual(typeof(BadRequestObjectResult), resultBad.GetType());
        }

        [TestMethod]
        public void UploadCsvTest()
        {
            var controller = new ValuesController(_tokenProvider.Object, _serviceProvider.Object);

            StreamReader readerCsv = new StreamReader("SampleData\\sample_data.csv");

            IFormFile fileCsv = new FormFile(readerCsv.BaseStream, 0, readerCsv.BaseStream.Length, "Data", "dummy.csv");

            var resultCsv = controller.UploadFile(fileCsv, FileType.CSV);
            Assert.AreEqual(typeof(OkObjectResult), resultCsv.GetType());

        }

        [TestMethod]
        public void QueryAndSortTestCase()
        {
            var controller = new ValuesController(_tokenProvider.Object, _serviceProvider.Object);

            StreamReader readerCsv = new StreamReader("SampleData\\sample_data.csv");

            IFormFile fileCsv = new FormFile(readerCsv.BaseStream, 0, readerCsv.BaseStream.Length, "Data", "dummy.csv");

            var resultCsv = controller.UploadFile(fileCsv, FileType.CSV);

            var sortParams = (new List<KeyValuePair<string, bool>>()
                {new KeyValuePair<string, bool>("ZipCode", false)}).ToDictionary(x => x.Key, x => x.Value);

            var filterResult = controller.QueryAndSort("Antalya", null,null);

            Assert.AreEqual(typeof(OkObjectResult), filterResult.GetType());

        }

        [TestMethod]
        public void QueryAndSortTestCase1()
        {
            var controller = new ValuesController(_tokenProvider.Object, _serviceProvider.Object);

            StreamReader readerCsv = new StreamReader("SampleData\\sample_data.csv");

            IFormFile fileCsv = new FormFile(readerCsv.BaseStream, 0, readerCsv.BaseStream.Length, "Data", "dummy.csv");

            var resultCsv = controller.UploadFile(fileCsv, FileType.CSV);

            var sortParams = (new List<KeyValuePair<string, bool>>()
                {new KeyValuePair<string, bool>("ZipCode", false)}).ToDictionary(x => x.Key, x => x.Value);

            var filterResult = controller.QueryAndSort("Antalya", null, FileType.XML);

            Assert.AreEqual(typeof(FileStreamResult), filterResult.GetType());
            Assert.AreEqual("text/xml", ((FileStreamResult)filterResult).ContentType);

        }

        [TestMethod]
        public void QueryAndSortTestCase2()
        {
            var controller = new ValuesController(_tokenProvider.Object, _serviceProvider.Object);

            StreamReader readerCsv = new StreamReader("SampleData\\sample_data.csv");

            IFormFile fileCsv = new FormFile(readerCsv.BaseStream, 0, readerCsv.BaseStream.Length, "Data", "dummy.csv");

            var resultCsv = controller.UploadFile(fileCsv, FileType.CSV);

            var sortParams = (new List<KeyValuePair<string, bool>>()
                {
                new KeyValuePair<string, bool>("CityName",
                    true),
                new KeyValuePair<string, bool>("DistrictName",
                    false)
            }).ToDictionary(x => x.Key,
                x => x.Value);

            var filterResult = controller.QueryAndSort(string.Empty, sortParams, FileType.CSV);

            Assert.AreEqual(typeof(FileStreamResult), filterResult.GetType());
            Assert.AreEqual("text/csv", ((FileStreamResult)filterResult).ContentType);

        }

        [TestMethod]
        public void QueryAndSortTestCase3()
        {
            var controller = new ValuesController(_tokenProvider.Object, _serviceProvider.Object);

            StreamReader readerCsv = new StreamReader("SampleData\\sample_data.xml");

            IFormFile fileCsv = new FormFile(readerCsv.BaseStream, 0, readerCsv.BaseStream.Length, "Data", "dummy.xml");

            var resultCsv = controller.UploadFile(fileCsv, FileType.XML);

            var sortParams = (new List<KeyValuePair<string, bool>>()
                {new KeyValuePair<string, bool>("ZipCode", false)}).ToDictionary(x => x.Key, x => x.Value);

            var filterResult = controller.QueryAndSort("Ankara", sortParams, FileType.CSV);

            Assert.AreEqual(typeof(FileStreamResult), filterResult.GetType());
            Assert.AreEqual("text/csv", ((FileStreamResult)filterResult).ContentType);

        }


    }
}
