using System;

namespace marti_tech_demo.Models
{
    /// <summary>
    /// Token veri modeli.
    /// </summary>
    public class TokenObject
    {
        public TokenObject(string token)
        {
            Key = token;
            CreationDate = DateTime.Now;
        }
        public string Key { get; set; }
        public DateTime CreationDate { get; set; }
    }
}