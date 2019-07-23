using System;
using Amazon.Runtime.Internal;
using Epsagon.Dotnet.Config;
using Epsagon.Dotnet.Instrumentation;
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
            _logger.LogDebug("Services registered, epsagon started");
        }

        public static void RegisterCustomizers()
        {
            RuntimePipelineCustomizerRegistry.Instance.Register(new EpsagonPipelineCustomizer());
        }
    }
}
