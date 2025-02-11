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

        [Test]
        public async Task VerifyThat10OldestOpenIssuesReturned()
        {
            var response = await _gitHubClient.PostGraphQLQueryAsync(_query);
            var data = await response.Content.ReadAsStringAsync();

            Assert.IsFalse(string.IsNullOrEmpty(data), "JSON response is empty.");

            var issueResult = JsonConvert.DeserializeObject<Response>(data);

            Assert.IsNotNull(issueResult);
            Assert.IsNotNull(issueResult.Data);
            Assert.IsNotNull(issueResult.Data.Repository);
            Assert.IsNotNull(issueResult.Data.Repository.Issues);
            Assert.IsNotNull(issueResult.Data.Repository.Issues.Nodes);
            Assert.IsTrue(issueResult.Data.Repository.Issues.Nodes.Count == 10, "Incorrect number of issues returned");
        }

        [Test]
        public async Task VerifyIssuesContainBasicDetails()
        {
            var response = await _gitHubClient.PostGraphQLQueryAsync(_query);
            var data = await response.Content.ReadAsStringAsync();

            Assert.IsFalse(string.IsNullOrEmpty(data), "JSON response is empty.");

            var issueResult = JsonConvert.DeserializeObject<Response>(data);

            int issues = issueResult.Data.Repository.Issues.Nodes.Count;

            for(int i = 0; i < issues; i++)
            {
                Assert.IsNotNull(issueResult.Data.Repository.Issues.Nodes[i].Title);
                Assert.IsNotNull(issueResult.Data.Repository.Issues.Nodes[i].Url);
                Assert.IsNotNull(issueResult.Data.Repository.Issues.Nodes[i].CreatedAt);
                Assert.IsNotNull(issueResult.Data.Repository.Issues.Nodes[i].Author);
                Assert.IsNotNull(issueResult.Data.Repository.Issues.Nodes[i].Author.Login);
                Assert.IsNotNull(issueResult.Data.Repository.Issues.Nodes[i].State);
            }
        }

        [Test]
        public async Task VerifyEachIssueShouldBeOpen()
        {
            var response = await _gitHubClient.PostGraphQLQueryAsync(_query);
            var data = await response.Content.ReadAsStringAsync();

            Assert.IsFalse(string.IsNullOrEmpty(data), "JSON response is empty.");

            var issueResult = JsonConvert.DeserializeObject<Response>(data);

            int issues = issueResult.Data.Repository.Issues.Nodes.Count;

            for (int i = 0; i < issues; i++)
            {
                string state = issueResult.Data.Repository.Issues.Nodes[i].State.ToString();
                Assert.That(state, Is.EqualTo("OPEN"));
            }
        }
    }
}
