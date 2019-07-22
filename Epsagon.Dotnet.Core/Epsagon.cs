using System;
using Epsagon.Dotnet.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Epsagon.Dotnet.Core
{
    public class EpsagonUtils
    {
        private static IServiceProvider _serviceProvider;
        
        public static T GetService<T>() => _serviceProvider.GetService<T>();
        public static ILogger<T> GetLogger<T>() => _serviceProvider.GetService<ILoggerFactory>().CreateLogger<T>();

        public static void RegisterServices()
        {
            _serviceProvider = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.AddConsole(opts => opts.DisableColors = true);
                    builder.AddDebug();
                })
                .AddSingleton<IConfigurationService, ConfigurationService>()
                .BuildServiceProvider();

            var _logger = EpsagonUtils.GetLogger<EpsagonUtils>();
            _logger.LogInformation("Services registered, epsagon started");
        }
    }
}
