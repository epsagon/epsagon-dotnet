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

            JaegerTracer.CreateTracer();
            CustomizePipeline();
            Utils.RegisterConfiguration(LoadConfiguration());

            Log.Debug("finished bootstraping epsagon");
        }

        private static void CustomizePipeline()
        {
            Log.Debug("customizing AWSSDK pipeline - START");
            RuntimePipelineCustomizerRegistry.Instance.Register(new EpsagonPipelineCustomizer());
            Log.Debug("customizing AWSSDK pipeline - FINISHED");
        }

        private static IEpsagonConfiguration LoadConfiguration()
        {
            Log.Debug("loading epsagon configuration - START");
            var config = configurationService.GetConfig();
            Log.Debug("loading epsagon configuration - FINISHED");
            Log.Debug("loaded configuration: {@config}", config);

            return config;
        }
    }
}
