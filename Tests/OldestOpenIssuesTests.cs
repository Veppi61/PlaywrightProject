using Newtonsoft.Json;
using PlaywrightProject.Models.RepoModels;

namespace PlaywrightProject.Tests
{
    [TestFixture]
    public class OldestOpenIssuesTests : BaseTest
    {
        private readonly string _query = @"
                query {
                  repository(owner: ""nodejs"", name: ""node"") {
                    issues(first: 10, states: OPEN, orderBy: {field: CREATED_AT, direction: ASC}) {
                      nodes {
                        title
                        url
                        createdAt
                        author {
                          login
                        }
                        state
                      }
                    }
                  }
                }";

        private async Task<Response> FetchIssuesDataAsync()
        {
            var response = await _gitHubClient.PostGraphQLQueryAsync(_query);
            var data = await response.Content.ReadAsStringAsync();

            Assert.IsFalse(string.IsNullOrEmpty(data), "JSON response is empty.");

            var issueResult = JsonConvert.DeserializeObject<Response>(data);
            Assert.NotNull(issueResult, "Failed to deserialize JSON response.");
            Assert.NotNull(issueResult.Data, "Response contains no 'data' field.");
            Assert.NotNull(issueResult.Data.Repository, "Response contains no 'repository' field.");
            Assert.NotNull(issueResult.Data.Repository.Issues, "Response contains no 'issues' field.");
            Assert.NotNull(issueResult.Data.Repository.Issues.Nodes, "Response contains no 'nodes' field.");

            return issueResult;
        }

        [Test]
        public async Task VerifyThat10OldestOpenIssuesReturned()
        {
            var issueResult = await FetchIssuesDataAsync();
            Assert.That(issueResult.Data.Repository.Issues.Nodes.Count, Is.EqualTo(10),
                $"Expected 10 issues, but got {issueResult.Data.Repository.Issues.Nodes.Count}.");
        }

        [Test]
        public async Task VerifyIssuesContainBasicDetails()
        {
            var issueResult = await FetchIssuesDataAsync();

            Assert.Multiple(() =>
            {
                foreach (var issue in issueResult.Data.Repository.Issues.Nodes)
                {
                    Assert.IsNotNull(issue.Title, "Issue title is null.");
                    Assert.IsNotNull(issue.Url, $"Issue '{issue.Title}' has no URL.");
                    Assert.IsNotNull(issue.CreatedAt, $"Issue '{issue.Title}' has no creation date.");
                    Assert.IsNotNull(issue.Author, $"Issue '{issue.Title}' has no author.");
                    Assert.IsNotNull(issue.Author.Login, $"Issue '{issue.Title}' has no author login.");
                    Assert.IsNotNull(issue.State, $"Issue '{issue.Title}' has no state.");
                }
            });
        }

        [Test]
        public async Task VerifyEachIssueShouldBeOpen()
        {
            var issueResult = await FetchIssuesDataAsync();

            Assert.Multiple(() =>
            {
                foreach (var issue in issueResult.Data.Repository.Issues.Nodes)
                {
                    Assert.That(issue.State, Is.EqualTo("OPEN"),
                        $"Issue '{issue.Title}' is in state '{issue.State}', expected 'OPEN'.");
                }
            });
        }
    }
}
