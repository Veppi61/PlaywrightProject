using Newtonsoft.Json;
using PlaywrightProject.Models.ConfigModels;
using System.Net.Http.Json;

namespace PlaywrightProject.Utils
{
    public class GitHubClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonModel _config;

        public GitHubClient()
        {
            _config = ReadConf();
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.GitHubToken}");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", _config.GitHubToken);
        }

        private JsonModel ReadConf()
        {
            string jsonData = File.ReadAllText("config.json");
            return JsonConvert.DeserializeObject<JsonModel>(jsonData);
        }

        public async Task<HttpResponseMessage> PostGraphQLQueryAsync(string query)
        {
            var response = await _httpClient.PostAsJsonAsync(_config.Url, new { query });
            response.EnsureSuccessStatusCode();
            return response;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
