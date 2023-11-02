using Refit;

namespace AteaScraper.Integrations
{
    public interface IPublicApiClient
    {
        [Get("/random")]
        Task<ApiResponse<PublicApiResponse>> GetRandomData(string? auth = null);
    }
}
