using AteaScraper.Integrations;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Refit;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AteaScraper
{
    public class Scraper
    {
        [FunctionName("Scraper")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var service = RestService.For<IPublicApiClient>("https://api.publicapis.org", new RefitSettings(new NewtonsoftJsonContentSerializer()));
            var data = await service.GetRandomData(null);

            var tableClient = new TableClient("UseDevelopmentStorage=true", "AteaFunctionsTest");
            await tableClient.CreateIfNotExistsAsync();
            var id = Guid.NewGuid().ToString().Replace("-", "");

            var entry = new LogEntry
            {
                PartitionKey = id.ToString(),
                RowKey = id.ToString(),
                Succeeded = data.IsSuccessStatusCode
            };

            await tableClient.AddEntityAsync(entry);

            var blobContent = JsonConvert.SerializeObject(data.Content);
            using var memoryStream = new MemoryStream();
            await using var streamWriter = new StreamWriter(memoryStream);
            await streamWriter.WriteAsync(blobContent);
            await streamWriter.FlushAsync();
            memoryStream.Position = 0;

            var blobContainerClient = new BlobContainerClient("UseDevelopmentStorage=true", "atea");
            _ = await blobContainerClient.CreateIfNotExistsAsync();

            var blobClient = new BlobClient("UseDevelopmentStorage=true", "atea", $"{id}.json");
            await blobClient.UploadAsync(memoryStream);
        }
    }
}
