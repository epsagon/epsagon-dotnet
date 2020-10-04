
using System;

namespace Epsagon.Dotnet.Instrumentation
{
    public static class EpsagonUtils
    {
        private static string traceUrl;

        public static void SetLambdaTraceUrl(string awsAccount,string awsRegion,string functionName,string awsRequestId) => 
            traceUrl = $"https://app.epsagon.com/functions/{awsAccount}/{awsRegion}/{functionName}?requestId={awsRequestId}";
        
        public static string GetTraceUrl() => String.IsNullOrEmpty(traceUrl) ? "" : traceUrl;
    }
}
