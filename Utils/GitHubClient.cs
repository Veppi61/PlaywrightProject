using Newtonsoft.Json;
using PlaywrightProject.Models.ConfigModels;
using System.Net.Http.Json;

namespace PlaywrightProject.Utils
{
    public class GitHubClient
    {
        private readonly HttpClient _httpClient;

        public GitHubClient()
        {
            string jsonData = File.ReadAllText("config.json");  
            var deserializedJsonData = JsonConvert.DeserializeObject<JsonModel>(jsonData);
            
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {deserializedJsonData.GitHubToken}");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", deserializedJsonData.GitHubToken);
        }

        public async Task<HttpResponseMessage> PostGraphQLQueryAsync(string query)
        {
            string jsonData = File.ReadAllText("config.json");
            var deserializedJsonData = JsonConvert.DeserializeObject<JsonModel>(jsonData);
            var response = await _httpClient.PostAsJsonAsync(deserializedJsonData.Url, new { query });
            response.EnsureSuccessStatusCode();
            return response;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
