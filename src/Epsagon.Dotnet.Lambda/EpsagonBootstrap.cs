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
                Utils.DebugLogIfEnabled("finished bootstraping epsagon");
            }
        }

        private static void CustomizePipeline()
        {
            Utils.DebugLogIfEnabled("customizing AWSSDK pipeline - START");
            RuntimePipelineCustomizerRegistry.Instance.Register(new EpsagonPipelineCustomizer());
            Utils.DebugLogIfEnabled("customizing AWSSDK pipeline - FINISHED");
        }

        private static IEpsagonConfiguration LoadConfiguration()
        {
            Utils.DebugLogIfEnabled("loading epsagon configuration - START");
            var config = configurationService.GetConfig();

            Utils.DebugLogIfEnabled("loading epsagon configuration - FINISHED");
            Utils.DebugLogIfEnabled("loaded configuration: {@config}", config);
            return config;
        }
    }
}
