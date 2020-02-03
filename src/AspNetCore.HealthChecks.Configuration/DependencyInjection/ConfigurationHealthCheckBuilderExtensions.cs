using AspNetCore.HealthChecks.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;

#if NETSTANDARD2_0
using Microsoft.Extensions.Configuration;
#endif

public static class ConfigurationHealthCheckBuilderExtensions
{
    private const string NAME = "configuration";

    public static IHealthChecksBuilder AddConfiguration(this IHealthChecksBuilder builder,
#if NETSTANDARD2_0
        IConfiguration configuration,
#endif
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
                sp => new ConfigurationHealthCheck(
                    configurationOptions
#if NETSTANDARD2_0
                    , configuration
#endif
                ),
                failureStatus,
                tags,
                timeout));
    }
}
