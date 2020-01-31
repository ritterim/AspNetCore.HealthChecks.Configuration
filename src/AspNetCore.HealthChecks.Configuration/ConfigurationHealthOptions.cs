using System;
using System.Collections.Generic;

namespace AspNetCore.HealthChecks.Configuration
{
    public class ConfigurationHealthOptions
    {
        public StringComparison StringComparison { get; set; } = StringComparison.OrdinalIgnoreCase;

        internal ICollection<string> MustNotContain { get; } = new HashSet<string>();

        public ConfigurationHealthOptions NotContains(string str)
        {
            MustNotContain.Add(str);

            return this;
        }
    }
}
