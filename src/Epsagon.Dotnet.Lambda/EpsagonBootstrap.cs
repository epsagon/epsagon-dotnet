using System;
using Amazon.Runtime.Internal;
using Epsagon.Dotnet.Core;
using Epsagon.Dotnet.Core.Configuration;
using Epsagon.Dotnet.Instrumentation;
using Epsagon.Dotnet.Tracing.OpenTracingJaeger;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Epsagon.Dotnet.Lambda
{
    public static class EpsagonBootstrap
    {
        private static readonly IConfigurationService configurationService = new ConfigurationService();

        public static void Bootstrap()
        {
            var levelSwitch = new LoggingLevelSwitch(LogEventLevel.Error);
            if ((System.Environment.GetEnvironmentVariable("EPSAGON_DEBUG") ?? "").ToLower() == "true")
            {
                levelSwitch.MinimumLevel = LogEventLevel.Debug;
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            if ((Environment.GetEnvironmentVariable("DISABLE_EPSAGON") ?? "").ToUpper() != "TRUE")
            {
                JaegerTracer.CreateTracer();
                CustomizePipeline();
                Utils.RegisterConfiguration(LoadConfiguration());

                if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
                {
                    Log.Debug("finished bootstraping epsagon");
                }
            }
        }

        private static void CustomizePipeline()
        {
            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
            {
                Log.Debug("customizing AWSSDK pipeline - START");
            }

            RuntimePipelineCustomizerRegistry.Instance.Register(new EpsagonPipelineCustomizer());

            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
            {
                Log.Debug("customizing AWSSDK pipeline - FINISHED");
            }
        }

        private static IEpsagonConfiguration LoadConfiguration()
        {
            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
            {
                Log.Debug("loading epsagon configuration - START");
            }
            var config = configurationService.GetConfig();

            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
            {
                Log.Debug("loading epsagon configuration - FINISHED");
                Log.Debug("loaded configuration: {@config}", config);
            }

            return config;
        }
    }
}
