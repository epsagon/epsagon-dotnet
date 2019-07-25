using System;
namespace Epsagon.Dotnet.Instrumentation
{
    public interface IFactory<TKey, TValue>
    {
         TValue GetInstace(TKey key);
    }
}
