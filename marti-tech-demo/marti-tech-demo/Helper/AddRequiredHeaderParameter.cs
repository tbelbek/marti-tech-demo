using System.Collections.Generic;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace marti_tech_demo.Helper
{
    /// <summary>
    /// Swagger header parametresi için konfigürasyon.
    /// </summary>
    public class AddRequiredHeaderParameter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<IParameter>();

            operation.Parameters.Add(new NonBodyParameter
            {
                Name = "Token",
                In = "header",
                Type = "string",
                Required = false
            });
        }
    }
}