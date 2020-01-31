# AspNetCore.HealthChecks.Configuration

A health check to verify configuration values.

| Package | Version |
| --- | -- |
| [AspNetCore.HealthChecks.Configuration][NuGet link] | [![AspNetCore.HealthChecks.Configuration NuGet Version](https://img.shields.io/nuget/v/AspNetCore.HealthChecks.Configuration.svg)][NuGet link] |

## Installation

Install the [AspNetCore.HealthChecks.Configuration][NuGet link] NuGet package.

```
> dotnet add package AspNetCore.HealthChecks.Configuration
```

or

```
PM> Install-Package AspNetCore.HealthChecks.Configuration
```

## Usage

```csharp

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddConfiguration(configuration, o => o.NotContains("abc"));
    }
}
```

## License

MIT License

[NuGet link]: https://www.nuget.org/packages/AspNetCore.HealthChecks.Configuration
