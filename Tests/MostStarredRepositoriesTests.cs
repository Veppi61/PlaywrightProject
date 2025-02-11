using Newtonsoft.Json;
using PlaywrightProject.Models.IssuesModel;

namespace PlaywrightProject.Tests
{
    [TestFixture]
    public class MostStarredRepositoriesTests : BaseTest
    {
        private readonly string _query = @"
                query {
                  search(query: ""stars:>=1"", type: REPOSITORY, first: 20) {
                    nodes {
                      ... on Repository {
                        name
                        owner {
                          login
                        }
                        stargazerCount
                        description
                        url
                      }
                    }
                  }
                }";

        [Test]
        public async Task VerifyThat20MostStaredRepositoriesReturned()
        {
            var response = await _gitHubClient.PostGraphQLQueryAsync(_query);
            var data = await response.Content.ReadAsStringAsync();

            Assert.IsFalse(string.IsNullOrEmpty(data), "JSON response is empty.");

            var result = JsonConvert.DeserializeObject<Root>(data);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsNotNull(result.Data.Search);
            Assert.IsNotNull(result.Data.Search.Nodes);
            Assert.That(result.Data.Search.Nodes.Count, Is.EqualTo(20), "Incorrect number of repositories returned");
        }

        [Test]
        public async Task VerifyRepositoriesContainBasicDetails()
        {
            var response = await _gitHubClient.PostGraphQLQueryAsync(_query);
            var data = await response.Content.ReadAsStringAsync();

            Assert.IsFalse(string.IsNullOrEmpty(data), "JSON response is empty.");

            var result = JsonConvert.DeserializeObject<Root>(data);

            int repositories = result.Data.Search.Nodes.Count;

            for (int i = 0; i < repositories; i++)
            {
                Assert.IsNotNull(result.Data.Search.Nodes[i].Name);
                Assert.IsNotNull(result.Data.Search.Nodes[i].Owner);
                Assert.IsNotNull(result.Data.Search.Nodes[i].Owner.Login);
                Assert.IsNotNull(result.Data.Search.Nodes[i].StargazerCount);
                Assert.IsNotNull(result.Data.Search.Nodes[i].Description);
                Assert.IsNotNull(result.Data.Search.Nodes[i].Url);
            }
        }

        [Test]
        public async Task VerifyEachRepositoryHasMoreThousandStars()
        {
            var response = await _gitHubClient.PostGraphQLQueryAsync(_query);
            var data = await response.Content.ReadAsStringAsync();

            Assert.IsFalse(string.IsNullOrEmpty(data), "JSON response is empty.");

            var result = JsonConvert.DeserializeObject<Root>(data);

            int repositories = result.Data.Search.Nodes.Count;

            for (int i = 0; i < repositories; i++)
            {
                int stars = (int)result.Data.Search.Nodes[i].StargazerCount;
                Assert.That(stars, Is.GreaterThan(1000));
            }
        }
    }
}
