using System;
namespace Epsagon.Dotnet.Core.Configuration
{
    public interface IConfigurationService
    {
        IEpsagonConfiguration GetConfig();
        IEpsagonConfiguration DefaultConfig();
        void SetConfig(IEpsagonConfiguration config);
    }
}
