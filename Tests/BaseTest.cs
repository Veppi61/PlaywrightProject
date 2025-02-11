using Microsoft.Playwright;
using PlaywrightProject.Utils;


namespace PlaywrightProject.Tests
{
    public class BaseTest
    {
        protected IPlaywright _playwright;
        protected IBrowser _browser;
        protected IBrowserContext _context;
        protected GitHubClient _gitHubClient;

        [SetUp]
        public async Task Setup()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync();
            _context = await _browser.NewContextAsync();
            _gitHubClient = new GitHubClient();
        }

        [TearDown]
        public async Task TearDown()
        {
            await _context.CloseAsync();
            await _browser.CloseAsync();
            _playwright.Dispose();
            _gitHubClient.Dispose();
        }
    }
}
