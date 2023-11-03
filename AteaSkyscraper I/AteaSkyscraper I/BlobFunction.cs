using System.IO;
using System.Net;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace AteaScraper
{
    public class BlobFunction
    {
        private readonly ILogger<BlobFunction> _logger;

        public BlobFunction(ILogger<BlobFunction> log)
        {
            _logger = log;
        }

        [FunctionName("BlobFunction")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        //[OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **id** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req) 
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string blobId = req.Query["id"];

            var blobClient = new BlobClient("UseDevelopmentStorage=true", "atea", $"{blobId}json");
            var content = await blobClient.DownloadContentAsync();

            return new OkObjectResult(content.Value.Content.ToString());
        }
    }
}

