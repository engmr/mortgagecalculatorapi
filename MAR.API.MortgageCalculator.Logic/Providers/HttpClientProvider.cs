using MAR.API.MortgageCalculator.Logic.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;

namespace MAR.API.MortgageCalculator.Logic.Providers
{
    public class HttpClientProvider : IHttpClientProvider
    {
        public HttpClient HttpClient { get; set; }
        public async Task<string> GetAsync(string url)
        {
            using (var client = GetHttpClient())
            {
                using (var response = await client.GetAsync(url))
                {
                    if (response != null && response.IsSuccessStatusCode && response.Content != null)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    return string.Empty;
                }
            }
        }

        private HttpClient GetHttpClient()
        {
            if (HttpClient == null)
            {
                var _httpClient = new HttpClient();
                //_httpClient.DefaultRequestHeaders.Accept.Clear();
                //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return _httpClient;
            }
            return HttpClient;
        }
    }
}
