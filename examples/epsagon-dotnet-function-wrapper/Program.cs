using System;
using System.Threading;
using Epsagon.Dotnet.Lambda;

namespace EpsagonDotnetFunction
{
    class Program
    {
        static void Main(string[] args)
        {
            EpsagonBootstrap.Bootstrap(useOpenTracingCollector: true);

            var invoker = new LambdaInvoker();
            var responseTask = invoker.InvokeLambda(args[0]);
            Console.WriteLine($"Lambda Result: {responseTask}");
        }
    }
}
