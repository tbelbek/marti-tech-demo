using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using marti_tech_demo.Models;

namespace marti_tech_demo.Helper
{
    /// <summary>
    /// Token veri sağlayıcısı
    /// </summary>
    public class TokenProvider : ITokenProvider
    {
        private readonly IConfiguration _configManager;

        private readonly int TokenTimeout;
        public List<TokenObject> Tokens { get; set; }

        public TokenProvider(IConfiguration configManager)
        {
            _configManager = configManager;
            TokenTimeout = _configManager.GetValue<int>(
                "TokenTimeout");

            Tokens = LoadTokens();
        }
        /// <summary>
        /// Basit bir token mekanizması için oluşturulup dosyaya yazılan tokenları validate eder.
        /// </summary>
        public bool TokenValidator(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var tokenData = Tokens.FirstOrDefault(t => t.Key == token);

            if (tokenData != null && DateTime.Now <= tokenData.CreationDate.AddMinutes(TokenTimeout)) return true;

            Tokens.Remove(tokenData);
            return false;

        }

        /// <summary>
        /// Token oluşturur ve dosyaya yazar.
        /// </summary>
        /// <returns>token döndürür.</returns>
        public string TokenGenerator()
        {
            Tokens = LoadTokens();

            var token = Guid.NewGuid().ToString("n").ToUpper();
            Tokens.Add(new TokenObject(token));

            WriteTokens();

            return token;
        }

        /// <summary>
        /// Yazılan tokenları yükler.
        /// </summary>
        /// <returns>Token listesi</returns>
        public List<TokenObject> LoadTokens()
        {
            List<TokenObject> items;

            if (!File.Exists("tokens.json")) File.Create("tokens.json").Dispose();

            using (StreamReader r = new StreamReader("tokens.json"))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<TokenObject>>(json);
                items?.RemoveAll(t => DateTime.Now <= t.CreationDate);
            }

            return items ?? new List<TokenObject>();
        }

        /// <summary>
        /// Token listesini diske yazar.
        /// </summary>
        public void WriteTokens()
        {
            using (StreamWriter file = File.CreateText("tokens.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, Tokens);
            }
        }

    }
}
