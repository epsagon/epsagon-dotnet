using System.Threading.Tasks;

using Amazon.Lambda.Core;

using Epsagon.Dotnet.Instrumentation;

namespace Epsagon.Dotnet.Lambda {
    public abstract class LambdaHandler<TEvent, TRes> {

        public LambdaHandler() {
            EpsagonBootstrap.Bootstrap();
        }

        /// <summary>
        /// in a derived class this handler function is the base for epsagon's
        /// handler function, write business logic in this function
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract Task<TRes> HandlerFunction(TEvent input, ILambdaContext context);

        /// <summary>
        /// Epsagon enabled lambda handler based on <see cref="HandlerFunction(TReq, ILambdaContext)"/>
        /// implemented in a derived class
        /// </summary>
        /// <param name="input">input event from AWS Lambda</param>
        /// <param name="context">lambda context</param>
        /// <returns></returns>
        private TRes EpsagonEnabledHandler(TEvent input, ILambdaContext context) {
            return EpsagonHandler.Handle(input, context, () => this.HandlerFunction(input, context)).Result;
        }

    }
}
