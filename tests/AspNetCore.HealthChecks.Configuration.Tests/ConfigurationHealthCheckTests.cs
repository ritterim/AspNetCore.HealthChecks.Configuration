using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCore.HealthChecks.Configuration.Tests
{
    public class ConfigurationHealthCheckTests
    {
        [Fact]
        public void add_health_check_when_properly_configured()
        {
            var configuration = GetConfiguration(new Dictionary<string, string>
            {
                {"Key1", "Value1"},
                {"Nested:Key1", "NestedValue1"},
                {"Nested:Key2", "NestedValue2"}
            });

            var services = new ServiceCollection();
            services.AddHealthChecks()
                .AddConfiguration(configuration);

            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            registration.Name.Should().Be("configuration");
            check.GetType().Should().Be(typeof(ConfigurationHealthCheck));
        }

        // Built from
        // https://stackoverflow.com/a/55497919/941536
        // answering
        // https://stackoverflow.com/questions/55497800/populate-iconfiguration-for-unit-tests
        private static IConfiguration GetConfiguration(Dictionary<string, string> configuration)
        {
            var builtConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(configuration)
                .Build();

            return builtConfiguration;
        }
    }
}
