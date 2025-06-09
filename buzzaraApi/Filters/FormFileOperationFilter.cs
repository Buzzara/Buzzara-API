using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace buzzaraApi.SwaggerFilters
{
    /// <summary>
    /// Permite que o Swashbuckle entenda endpoints que usam [FromForm] IFormFile
    /// </summary>
    public class FormFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileParams = context.ApiDescription.ParameterDescriptions
                .Where(p => p.ModelMetadata?.ModelType == typeof(Microsoft.AspNetCore.Http.IFormFile));

            if (!fileParams.Any())
                return;
            operation.RequestBody = new OpenApiRequestBody
            {
                Content =
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = fileParams.ToDictionary(
                                p => p.Name,
                                p => new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary"
                                }
                            ),
                            Required = fileParams.Select(x => x.Name).ToHashSet()
                        }
                    }
                }
            };
        }
    }
}
