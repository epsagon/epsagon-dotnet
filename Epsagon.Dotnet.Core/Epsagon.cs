using System;
using System.Reflection;
using Epsagon.Dotnet.Core.Configuration;
using Epsagon.Dotnet.Tracing.OpenTracingJaeger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using OpenTracing;

using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Epsagon.Dotnet.Core
{
    public class EpsagonUtils
    {
        private static IServiceProvider _serviceProvider;

        public static T GetService<T>() => _serviceProvider.GetService<T>();
        public static ILogger<T> GetLogger<T>() => _serviceProvider.GetService<ILoggerFactory>().CreateLogger<T>();
        public static ILogger GetLogger(Type t) => _serviceProvider.GetService<ILoggerFactory>().CreateLogger(t);

        public static IEpsagonConfiguration GetConfiguration(Type handler)
        {
            var configService = GetService<IConfigurationService>();
            var epsagon = handler.GetCustomAttribute<EpsagonConfiguration>();

            if (epsagon != null)
            {
                configService.SetConfig(epsagon);
            }

            return configService.GetConfig();
        }

        public static void RegisterServices()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            _serviceProvider = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Debug);
                    builder.AddSerilog(dispose: true);
                })
                .AddSingleton<IConfigurationService, ConfigurationService>()
                .AddSingleton<ITracer, Jaeger.Tracer>((serviceProvider) => JaegerTracer.CreateTracer(serviceProvider.GetService<ILoggerFactory>()))
                .BuildServiceProvider();

            var _logger = EpsagonUtils.GetLogger<EpsagonUtils>();
            _logger.LogDebug("Services registered, epsagon started");
        }
    }
}
