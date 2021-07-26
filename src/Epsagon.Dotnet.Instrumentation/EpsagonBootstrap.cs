using System;
using System.Runtime.InteropServices;

using Amazon.Runtime.Internal;

using Epsagon.Dotnet.Core;
using Epsagon.Dotnet.Core.Configuration;
using Epsagon.Dotnet.Tracing.OpenTracingJaeger;

using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Epsagon.Dotnet.Instrumentation {
    public static class EpsagonBootstrap {
        private static readonly IConfigurationService configurationService = new ConfigurationService();

        public static void Bootstrap(bool useOpenTracingCollector = false, IEpsagonConfiguration configuration = null) {
            var levelSwitch = new LoggingLevelSwitch(LogEventLevel.Error);
            if ((Environment.GetEnvironmentVariable("EPSAGON_DEBUG") ?? "").ToLower() == "true") {
                levelSwitch.MinimumLevel = LogEventLevel.Debug;
            }

            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .Enrich.FromLogContext()
                .WriteTo.Console();
            // if ((configuration?.EnableEventLog).GetValueOrDefault(false) && RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            //     loggerConfig.WriteTo.EventLog("Epsagon");
            // }

            // var logConfig = configuration?.LogFile ?? Environment.GetEnvironmentVariable("EPSAGON_LOG_FILE");
            // if (!string.IsNullOrWhiteSpace(logConfig)) {
            //     loggerConfig.WriteTo.File(logConfig);
            // }

            Log.Logger = loggerConfig.CreateLogger();

            if ((Environment.GetEnvironmentVariable("DISABLE_EPSAGON") ?? "").ToUpper() != "TRUE") {
                if (configuration != null) { Utils.RegisterConfiguration(configuration); } else { Utils.RegisterConfiguration(LoadConfiguration()); }
                CustomizePipeline();

                // Use either legacy tracer or opentracing tracer
                if (useOpenTracingCollector) {
                    Utils.DebugLogIfEnabled("remote");
                    JaegerTracer.CreateRemoteTracer();
                } else
                    JaegerTracer.CreateTracer();
                Utils.DebugLogIfEnabled("finished bootstraping epsagon");
            }
        }

        private static void CustomizePipeline() {
            Utils.DebugLogIfEnabled("customizing AWSSDK pipeline - START");
            RuntimePipelineCustomizerRegistry.Instance.Register(new EpsagonPipelineCustomizer());
            Utils.DebugLogIfEnabled("customizing AWSSDK pipeline - FINISHED");
        }

        private static IEpsagonConfiguration LoadConfiguration() {
            Utils.DebugLogIfEnabled("loading epsagon configuration - START");
            var config = configurationService.GetConfig();

            Utils.DebugLogIfEnabled("loading epsagon configuration - FINISHED");
            Utils.DebugLogIfEnabled("loaded configuration: {@config}", config);
            return config;
        }
    }
}
