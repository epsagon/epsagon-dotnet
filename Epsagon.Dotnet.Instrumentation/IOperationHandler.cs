using System;
using OpenTracing;
using Amazon.Runtime;

namespace Epsagon.Dotnet.Instrumentation
{
    public interface IOperationHandler<out TReq, out TRes> 
        where TReq : AmazonWebServiceRequest 
        where TRes : AmazonWebServiceResponse
    {
        TReq Reqest { get; }
        TRes Response { get; }
        void HandleOperationBefore(IScope scope);
        void HandleOperationAfter(IScope scope);
    }
}
