namespace Epsagon.Dotnet.Tracing.Legacy
{
    public class EpsagonMetadata
    {
        public string AwsAccount { get; set; }
        public bool ColdStart { get; set; }
        public string FunctionVersion { get; set; }
        public string LogGroupName { get; set; }
        public string LogStreamName { get; set; }
        public string Memory { get; set; }
        public string Region { get; set; }
        public string ReturnValue { get; set; }
    }
}
