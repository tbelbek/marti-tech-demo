using System;
using System.Threading.Tasks;
using marti_tech_demo.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace marti_tech_demo.Filters
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private ITokenProvider _tokenValidateHelper;
        private IConfiguration configuration;

        public TokenMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            this.configuration = configuration;
        }

        /// <summary>
        /// Gelen request'i yakalar, ilgili header'ı kontrol eder. Geçerliyse 200, değilse 401 döndürür.
        /// </summary>
        /// <param name="httpContext">Request</param>
        /// <param name="tokenValidateHelper">Token validate işlemlerini içerir</param>
        /// <returns>Request</returns>
        public async Task Invoke(HttpContext httpContext, ITokenProvider tokenValidateHelper)
        {
            var token = httpContext.Request.Headers["Token"];

            bool tokenResult = false;

            try
            {
                _tokenValidateHelper = tokenValidateHelper;

                var request = httpContext.Request;

                if (!tokenValidateHelper.TokenValidator(token) && !request.Path.StartsWithSegments(new PathString("/swagger")))
                
                {
                    httpContext.Response.StatusCode = 401; //Unauthorized
                    return;
                }

                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await _next(httpContext).ConfigureAwait(true);
            }
        }
    }
}