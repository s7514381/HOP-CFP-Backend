using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace HOP_CFP_Backend.Utility
{
    public class LineNotifySender
    {
        protected IHttpClientFactory _httpClientFactory;
        protected string _token;

        public LineNotifySender(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _token = configuration.GetValue<string>("NLog:LineNotifyToken");
        }

        public async Task SentAsync(string message)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_token}");
            var content = new Dictionary<string, string>
            {
                { "message", $"\n{message}" }
            };
            await httpClient.PostAsync("https://notify-api.line.me/api/notify", new FormUrlEncodedContent(content));
        }
    }
}
