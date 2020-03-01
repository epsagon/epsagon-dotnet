using System;
using Epsagon.Dotnet.Lambda;

namespace EpsagonDotnetFunction
{
    class Program
    {
        static void Main(string[] args)
        {
            EpsagonBootstrap.Bootstrap();

            var invoker = new LambdaInvoker();
            var responseTask = invoker.InvokeLambda(args[0]);
            Console.WriteLine($"Lambda Result: {responseTask}");
        }
    }
}
