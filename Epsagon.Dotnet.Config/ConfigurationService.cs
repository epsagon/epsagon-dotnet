using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Epsagon.Dotnet.Config
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly ILogger<ConfigurationService> _logger;
        private static EpsagonConfiguration _config = new EpsagonConfiguration();

        public ConfigurationService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ConfigurationService>();
        }

        /// <summary>
        /// Return the current config instance used by epsagon
        /// </summary>
        /// <returns>current config instance</returns>
        public IEpsagonConfiguration GetConfig()
        {
            _logger.LogDebug("Accessing Config");
            return _config;
        }

        public IEpsagonConfiguration DefaultConfig() => new EpsagonConfiguration();

        /// <summary>
        /// Set a config instance to be used by epsagon,
        /// config values are merged with current values
        /// so you can omit options you don't want to change
        /// </summary>
        /// <param name="config">new config to use</param>
        public void SetConfig(IEpsagonConfiguration config)
        {
            _logger.LogDebug("Setting new config");
            _config.Token = this.SelectProp(config.Token, _config.Token);
            _config.AppName = this.SelectProp(config.AppName, _config.AppName);
            _config.MetadataOnly = this.SelectProp(config.MetadataOnly, _config.MetadataOnly);
            _config.UseSSL = this.SelectProp(config.UseSSL, _config.UseSSL);
            _config.TraceCollectorURL = this.SelectProp(config.TraceCollectorURL, _config.TraceCollectorURL);
            _config.IsEpsagonDisabled = this.SelectProp(config.IsEpsagonDisabled, _config.IsEpsagonDisabled);
        }

        public IEpsagonConfiguration FromAttribute(EpsagonAttribute attr)
        {
            return new EpsagonConfiguration()
            {
                AppName = attr.AppName,
                Token = attr.Token,
                IsEpsagonDisabled = attr.IsEpsagonDisabled,
                MetadataOnly = attr.MetadataOnly,
                TraceCollectorURL = attr.TraceCollectorURL,
                UseSSL = attr.UseSSL
            };
        }

        private T SelectProp<T>(T first, T second)
        {
            // check for null value
            if (EqualityComparer<T>.Default.Equals(first, default(T)))
            {
                return second;
            }

            if (!first.Equals(second))
            {
                return first;
            }

            return second;
        }
    }
}
