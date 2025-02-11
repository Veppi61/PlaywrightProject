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

        private async Task<Root> FetchRepositoriesDataAsync()
        {
            var response = await _gitHubClient.PostGraphQLQueryAsync(_query);
            var data = await response.Content.ReadAsStringAsync();

            Assert.IsFalse(string.IsNullOrEmpty(data), "JSON response is empty.");

            var result = JsonConvert.DeserializeObject<Root>(data);
            Assert.NotNull(result, "Failed to deserialize JSON response.");
            Assert.NotNull(result.Data, "Response contains no 'data' field.");
            Assert.NotNull(result.Data.Search, "Response contains no 'search' field.");
            Assert.NotNull(result.Data.Search.Nodes, "Response contains no 'nodes' field.");

            return result;
        }

        [Test]
        public async Task VerifyThat20MostStarredRepositoriesReturned()
        {
            var result = await FetchRepositoriesDataAsync();
            Assert.That(result.Data.Search.Nodes.Count, Is.EqualTo(20), "Incorrect number of repositories returned.");
        }

        [Test]
        public async Task VerifyRepositoriesContainBasicDetails()
        {
            var result = await FetchRepositoriesDataAsync();

            Assert.Multiple(() =>
            {
                foreach (var repo in result.Data.Search.Nodes)
                {
                    Assert.IsNotNull(repo.Name, "Repository name is null.");
                    Assert.IsNotNull(repo.Owner, "Repository owner is null.");
                    Assert.IsNotNull(repo.Owner.Login, $"Repository {repo.Name} has no owner login.");
                    Assert.IsNotNull(repo.StargazerCount, $"Repository {repo.Name} has no star count.");
                    Assert.IsNotNull(repo.Description, $"Repository {repo.Name} has no description.");
                    Assert.IsNotNull(repo.Url, $"Repository {repo.Name} has no URL.");
                }
            });
        }

        [Test]
        public async Task VerifyEachRepositoryHasMoreThanThousandStars()
        {
            var result = await FetchRepositoriesDataAsync();

            Assert.Multiple(() =>
            {
                foreach (var repo in result.Data.Search.Nodes)
                {
                    Assert.That(repo.StargazerCount, Is.GreaterThan(1000),
                        $"Repository {repo.Name} has only {repo.StargazerCount} stars.");
                }
            });
        }
    }
}
