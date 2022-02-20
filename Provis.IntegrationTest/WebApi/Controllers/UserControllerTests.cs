using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Provis.Infrastructure.Data;
using Provis.IntegrationTest.Base;
using Provis.IntegrationTest.Data;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Provis.IntegrationTest.WebApi.Controllers
{
    [TestFixture]
    public class UserControllerTests
    {
        protected ApiWebApplicationFactory _factory;
        protected HttpClient _httpClient;
        protected ProvisDbContext _dbContext;

        protected string _currentUserId;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _factory = new ApiWebApplicationFactory();

            _httpClient = _factory.WithWebHostBuilder(builer =>
            {
                builer.ConfigureServices(services =>
                {
                    services.AddMetrics();
                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                });
            }).CreateClient();

            _dbContext = _factory.Services.CreateScope().ServiceProvider
                .GetRequiredService<ProvisDbContext>();

            DatabaseInitialization.InitializeDatabase(_dbContext);

            _currentUserId = UsersData.currentUserId;

            FakePolicyEvaluator.claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _currentUserId)
            };
        }

        //[Test]
        public async Task GetUserPersonalInfoAsync_EndpointReturnSuccess()
        {
            var response = await _httpClient.GetAsync("api/user/info");

            response.Should()
                .NotBeNull();

            response.IsSuccessStatusCode.Should()
                .BeTrue();

            response.Content.Headers.ContentType.ToString().Should()
                .Be(ContentTypes.applicationJson);
        }
    }
}
