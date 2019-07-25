using System;
using System.Collections.Generic;
using Epsagon.Dotnet.Instrumentation.Handlers;
using Epsagon.Dotnet.Instrumentation.Handlers.S3;

namespace Epsagon.Dotnet.Instrumentation
{
    public class HandlerFactory : IFactory<Type, IServiceHandler>
    {
        public static Dictionary<Type, Func<IServiceHandler>> constructors = new Dictionary<Type, Func<IServiceHandler>>
        {
            { typeof(Amazon.S3.AmazonS3Client), () => new SimpleServiceHandler<S3OperationsFactory>() },
        };

        public IServiceHandler GetInstace(Type key)
        {
            return constructors[key]();
        }
    }
}
