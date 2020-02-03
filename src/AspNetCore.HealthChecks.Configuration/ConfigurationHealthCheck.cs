using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#if NETSTANDARD2_0
using Microsoft.Extensions.Configuration;
#endif

#if NET462
using System.Configuration;
#endif

namespace AspNetCore.HealthChecks.Configuration
{
    public class ConfigurationHealthCheck : IHealthCheck
    {
        private readonly ConfigurationHealthOptions configurationOptions;
#if NETSTANDARD2_0
        private readonly IConfiguration configuration;
#endif

        public ConfigurationHealthCheck(
            ConfigurationHealthOptions configurationOptions
#if NETSTANDARD2_0
            , IConfiguration configuration
#endif
        )
        {
            this.configurationOptions = configurationOptions ?? throw new ArgumentNullException(nameof(configurationOptions));
#if NETSTANDARD2_0
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
#endif
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var failures = new List<string>();

#if NETSTANDARD2_0
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
#endif
#if NET462
            // For usage with https://github.com/ritterim/RimDev.AspNet.Diagnostics.HealthChecks

            foreach (var key in ConfigurationManager.AppSettings.AllKeys)
            {
                var value = ConfigurationManager.AppSettings[key];

                foreach (var mustNotContainValue in configurationOptions.MustNotContain)
                {
                    if (value != null && value.IndexOf(mustNotContainValue, configurationOptions.StringComparison) != -1)
                    {
                        failures.Add($"App setting '{key}' must not contain '{mustNotContainValue}.'");
                    }
                }
            }

            foreach (ConnectionStringSettings setting in ConfigurationManager.ConnectionStrings)
            {
                var value = ConfigurationManager.ConnectionStrings[setting.Name];

                foreach (var mustNotContainValue in configurationOptions.MustNotContain)
                {
                    if (value != null && value.ConnectionString.IndexOf(mustNotContainValue, configurationOptions.StringComparison) != -1)
                    {
                        failures.Add($"Connection string '{setting.Name}' must not contain '{mustNotContainValue}.'");
                    }
                }
            }
#endif

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
