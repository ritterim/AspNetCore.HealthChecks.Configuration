using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

#if NETCOREAPP3_1
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
#endif

namespace AspNetCore.HealthChecks.Configuration.Tests
{
    public class ConfigurationHealthCheckTests
    {
#if NETCOREAPP3_1
        [Fact]
        public void add_health_check_when_properly_configured()
        {
            var configuration = GetConfiguration(new Dictionary<string, string>());

            var services = new ServiceCollection();
            services.AddHealthChecks()
                .AddConfiguration(configuration, o => { });

            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            registration.Name.Should().Be("configuration");
            check.GetType().Should().Be(typeof(ConfigurationHealthCheck));
        }

        [Fact]
        public async Task should_pass_when_value_does_not_exist_that_should_not_exist()
        {
            var configuration = GetConfiguration(new Dictionary<string, string>
            {
                {"Key1", "Value1"},
                {"Nested:Key1", "NestedValue1"}
            });

            var services = new ServiceCollection();
            services.AddHealthChecks()
                .AddConfiguration(configuration, o => o.NotContains("abc"));

            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            var result = await check.CheckHealthAsync(null);

            result.Status.Should().Be(HealthStatus.Healthy);
        }

        [Fact]
        public async Task should_fail_when_value_exists_when_should_not_exist()
        {
            var configuration = GetConfiguration(new Dictionary<string, string>
            {
                {"Key1", "abc"},
                {"Nested:Key1", "NestedValue1"}
            });

            var services = new ServiceCollection();
            services.AddHealthChecks()
                .AddConfiguration(configuration, o => o.NotContains("abc"));

            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            var result = await check.CheckHealthAsync(null);

            result.Status.Should().Be(HealthStatus.Unhealthy);
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
#endif

#if NET462
        [Fact]
        public void add_health_check_when_properly_configured()
        {
            var services = new ServiceCollection();
            services.AddHealthChecks()
                .AddConfiguration(o => { });

            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            registration.Name.Should().Be("configuration");
            check.GetType().Should().Be(typeof(ConfigurationHealthCheck));
        }

        [Fact]
        public async Task should_pass_when_value_does_not_exist_that_should_not_exist()
        {
            var services = new ServiceCollection();
            services.AddHealthChecks()
                .AddConfiguration(o => o.NotContains("abc"));

            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            var result = await check.CheckHealthAsync(null);

            result.Status.Should().Be(HealthStatus.Healthy);
        }

#pragma warning disable xUnit1004 // Test methods should not be skipped
        [Fact(Skip = "Manually edit App.config to test this.")]
#pragma warning restore xUnit1004 // Test methods should not be skipped
        public async Task should_fail_when_value_exists_when_should_not_exist()
        {
            var services = new ServiceCollection();
            services.AddHealthChecks()
                .AddConfiguration(o => o.NotContains("abc"));

            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            var result = await check.CheckHealthAsync(null);

            result.Status.Should().Be(HealthStatus.Unhealthy);
        }
#endif
    }
}
