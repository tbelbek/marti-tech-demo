using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace marti_tech_demo.Helper
{
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
            Tokens = new List<TokenObject>();
        }
        /// <summary>
        /// Basit bir token mekanizması
        /// </summary>


        public bool TokenValidator(string token)
        {
            var tokenData = Tokens.FirstOrDefault(t => t.Key == token);

            if (DateTime.Now <= tokenData.CreationDate) return true;

            Tokens.Remove(tokenData);
            return false;

        }

        public string TokenGenerator()
        {
            var token = Guid.NewGuid().ToString("n").ToUpper();
            Tokens.Add(new TokenObject(token));
            return token;
        }
    }

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
