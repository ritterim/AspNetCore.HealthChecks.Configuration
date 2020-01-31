using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.HealthChecks.Configuration
{
    public class ConfigurationHealthCheck : IHealthCheck
    {
        private readonly ConfigurationHealthOptions configurationOptions;
        private readonly IConfiguration configuration;

        public ConfigurationHealthCheck(
            ConfigurationHealthOptions configurationOptions,
            IConfiguration configuration)
        {
            this.configurationOptions = configurationOptions ?? throw new ArgumentNullException(nameof(configurationOptions));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var failures = new List<string>();

            foreach (var child in configuration.GetChildren())
            {
                foreach (var mustNotContainValue in configurationOptions.MustNotContain)
                {
                    if (child.Value != null && child.Value.IndexOf(mustNotContainValue, configurationOptions.StringComparison) != -1)
                    {
                        failures.Add($"{child.Key} must not contain '{mustNotContainValue}.'");
                    }
                }
            }

            if (failures.Any())
            {
                return Task.FromResult(
                    new HealthCheckResult(
                        HealthStatus.Unhealthy,
                        description: string.Join(Environment.NewLine, failures)));
            }

            return Task.FromResult(
                HealthCheckResult.Healthy());
        }
    }
}
