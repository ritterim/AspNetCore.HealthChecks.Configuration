using AspNetCore.HealthChecks.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;

public static class ConfigurationHealthCheckBuilderExtensions
{
    private const string NAME = "configuration";

    public static IHealthChecksBuilder AddConfiguration(this IHealthChecksBuilder builder,
        IConfiguration configuration,
        Action<ConfigurationHealthOptions> setup,
        string name = default,
        HealthStatus? failureStatus = default,
        IEnumerable<string> tags = default,
        TimeSpan? timeout = default)
    {
        var configurationOptions = new ConfigurationHealthOptions();
        setup?.Invoke(configurationOptions);

        return builder.Add(new HealthCheckRegistration(
                name ?? NAME,
                sp => new ConfigurationHealthCheck(configurationOptions, configuration),
                failureStatus,
                tags,
                timeout));
    }
}
