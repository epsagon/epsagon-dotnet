
namespace Epsagon.Dotnet.Tracing.Legacy
{
    public interface ITraceSender
    {
        void SendTrace(EpsagonTrace trace);
    }
}
