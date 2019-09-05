namespace Epsagon.Dotnet.Tracing.Legacy
{
    public class EpsagonEvent
    {
        public double StartTime { get; set; }
        public double Duration { get; set; }
        public int ErrorCode { get; set; }
        public string Id { get; set; }
        public string Origin { get; set; }
        public EpsagonResource Resource { get; set; }
        public EpsagonException Exception { get; set; }

        public EpsagonEvent(
            double startTime,
            double duration,
            int errorCode,
            string id,
            string origin,
            EpsagonResource resource,
            EpsagonException exception = null
        ) {
            this.StartTime  = startTime;
            this.Duration = duration;
            this.ErrorCode = errorCode;
            this.Id = id;
            this.Origin = origin;
            this.Resource = resource;
            this.Exception = exception;
        }
    }
}
