using System.Collections.Generic;
using marti_tech_demo.Models;
using Microsoft.AspNetCore.Http;

namespace marti_tech_demo.Helper
{
    /// <summary>
    /// Dosya içindeki verileri veri modeline çevirmek için gereken metodları içerir.
    /// </summary>
    public interface IFileParser
    {
        IEnumerable<City> ParseFile(IFormFile file);
    }
}