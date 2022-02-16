using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Provis.WebApi;
using App.Metrics.Formatters.Prometheus;

namespace Provis.IntegrationTest.Base
{
    public class ApiWebApplicationFactory: WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config =>
            {
                var integrationConfig = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.Testing.json")
                    .Build();

                config.AddConfiguration(integrationConfig);
            });
            builder
                .UseMetricsEndpoints(options =>
                {
                    options.EnvironmentInfoEndpointEnabled = false;
                    options.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
                    options.MetricsEndpointOutputFormatter = new MetricsPrometheusProtobufOutputFormatter();
                });
            builder.UseEnvironment("Testing");
        }
    }
}
