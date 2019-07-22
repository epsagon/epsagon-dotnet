using System;
namespace Epsagon.Dotnet.Config
{
    public interface IConfigurationService
    {
        IEpsagonConfiguration GetConfig();
        IEpsagonConfiguration DefaultConfig();
        IEpsagonConfiguration FromAttribute(EpsagonAttribute attr);
        void SetConfig(IEpsagonConfiguration config);
    }
}
