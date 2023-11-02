using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AteaScraper
{
    public class ListLogs
    {
        private readonly ILogger<ListLogs> _logger;

        public ListLogs(ILogger<ListLogs> log)
        {
            _logger = log;
        }

        [FunctionName("ListLogs")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiParameter(name: "from", In = ParameterLocation.Query, Required = true, Type = typeof(DateTime), Description = "The **from** parameter")]
        [OpenApiParameter(name: "to", In = ParameterLocation.Query, Required = true, Type = typeof(DateTime), Description = "The **to** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var from = DateTime.Parse(req.Query["from"]);
            var to = DateTime.Parse(req.Query["to"]);

            var tableClient = new TableClient("UseDevelopmentStorage=true", "AteaFunctionsTest");
            await tableClient.CreateIfNotExistsAsync();

            var logs = tableClient.Query<LogEntry>(e => e.Timestamp >= from && e.Timestamp <= to);

            return new OkObjectResult(logs.ToList());
        }
    }
}

