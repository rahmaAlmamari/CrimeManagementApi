using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CrimeManagementApi.Swagger
{
    /// <summary>
    /// Ensures multipart/form-data file upload fields display correctly in Swagger UI.
    /// </summary>
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileParams = context.MethodInfo
                .GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile)
                         || (p.ParameterType.IsClass &&
                             p.ParameterType.GetProperties()
                             .Any(pp => pp.PropertyType == typeof(IFormFile))))
                .ToList();

            if (!fileParams.Any()) return;

            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = fileParams
                                .SelectMany(p => p.ParameterType.GetProperties())
                                .ToDictionary(
                                    prop => prop.Name,
                                    prop => prop.PropertyType == typeof(IFormFile)
                                        ? new OpenApiSchema { Type = "string", Format = "binary" }
                                        : new OpenApiSchema { Type = "string" })
                        }
                    }
                }
            };
        }
    }
}
