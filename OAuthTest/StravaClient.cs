namespace OAuthTest
{
    public class StravaClient
    {
        private readonly IHttpClientFactory httpClientFactory;

        public StravaClient(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

    }
}
