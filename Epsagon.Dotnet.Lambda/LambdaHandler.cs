using Amazon.Lambda.Core;
using Epsagon.Dotnet.Core;
using Epsagon.Dotnet.Instrumentation;

namespace Epsagon.Dotnet.Lambda
{
    public abstract class LambdaHandler<TReq, TRes>
    {
        public LambdaHandler() : base()
        {
            EpsagonUtils.RegisterServices();
            EpsagonPipelineCustomizer.PatchPipeline();
        }

        /// <summary>
        /// in a derived class this handler function is the base for epsagon's
        /// handler function, write business logic in this function
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract TRes HandlerFunction(TReq input, ILambdaContext context);

        /// <summary>
        /// Epsagon enabled lambda handler based on <see cref="HandlerFunction(TReq, ILambdaContext)"/>
        /// implemented in a derived class
        /// </summary>
        /// <param name="input">input event from AWS Lambda</param>
        /// <param name="context">lambda context</param>
        /// <returns></returns>
        private TRes EpsagonEnabledHandler(TReq input, ILambdaContext context)
        {
            var config = EpsagonUtils.GetConfiguration(GetType());
            return this.HandlerFunction(input, context);
        }
    }
}
