using FluentAssertions;
using Refit;

namespace AteaScraper.Integrations.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var service = RestService.For<IPublicApiClient>("https://api.publicapis.org", new RefitSettings(new NewtonsoftJsonContentSerializer()));
            var result = await service.GetRandomData();

            result.IsSuccessStatusCode.Should().BeTrue();
        }
    }
}