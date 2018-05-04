using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace JapanDictionary.Common
{
    public class ApiHelper
    {
        private readonly HttpClient _httpClient;

        public ApiHelper()
        {
            _httpClient = new HttpClient(new HttpClientHandler
            {
                CookieContainer = new CookieContainer(),
            });

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            _httpClient.Timeout = TimeSpan.FromSeconds(300);
        }

        public async Task<string> GetAsync(string url)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await _httpClient.SendAsync(httpRequestMessage);
            if (!response.IsSuccessStatusCode)
                return string.Empty;

            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
    }
}
