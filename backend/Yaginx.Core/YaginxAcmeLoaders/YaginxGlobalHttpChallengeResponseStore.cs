using AgileLabs;
using AgileLabs.ApiClients;
using LettuceEncrypt.Internal;
using Microsoft.Extensions.Logging;
using System.Web;

namespace LettuceEncrypt.YaginxAcmeLoaders
{
    public class YaginxGlobalHttpChallengeResponseStore : ApiClient, IHttpChallengeResponseStore
    {
        private readonly IHttpClientFactory httpClientFactory;

        public YaginxGlobalHttpChallengeResponseStore(IHttpClientFactory httpClientFactory, ILogger<YaginxGlobalHttpChallengeResponseStore> logger) : base(logger)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public void AddChallengeResponse(string token, string response)
        {
            GetAsync($"/api/key_value/set?key={HttpUtility.UrlEncode(token)}&value={HttpUtility.UrlEncode(response)}").Wait();
        }

        public bool TryGetResponse(string token, out string? value)
        {
            var result = GetAsync<EnvelopMessage<string>>($"/api/key_value/get?key={HttpUtility.UrlEncode(token)}").Result;
            if (result.Code == 200)
            {
                value = result.Data;
            }
            else
            {
                value = string.Empty;
            }
            return value.IsNotNullOrWhitespace();
        }

        protected override HttpClient CreateHttpClient()
        {
            var httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("https://api.niusys.com");
            return httpClient;
        }
    }
}
