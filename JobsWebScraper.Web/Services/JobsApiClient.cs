namespace JobsWebScraper.Web.Services
{
    /// <summary>
    /// Talks to the JobsWebScraper API. The jobs grid reads the database directly, so this
    /// exists only for work that lives in the backend -- currently triggering the scrapers.
    /// </summary>
    public class JobsApiClient
    {
        private readonly HttpClient _http;

        public JobsApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task TriggerScrapeAsync(CancellationToken cancellationToken = default)
        {
            var response = await _http.PostAsync("api/automation/all", null, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
