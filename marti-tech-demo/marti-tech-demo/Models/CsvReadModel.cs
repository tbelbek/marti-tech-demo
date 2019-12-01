namespace marti_tech_demo.Models
{
    /// <summary>
    /// CSV parse edildikten sonra oluşan veri modeli. Aynı zamanda uygulama içindeki filter mekanizmasında da kullanılır.
    /// </summary>
    public class CsvReadModel
    {
        public string CityName { get; set; }
        public int CityCode { get; set; }
        public string DistrictName { get; set; }
        public int ZipCode { get; set; }


    }
}