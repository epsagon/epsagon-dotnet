using System;
using Amazon.Lambda.Core;
using Epsagon.Dotnet.Config;
using Epsagon.Dotnet.Core;
using Microsoft.Extensions.Logging;

namespace Epsagon.Dotnet.Lambda
{
    public abstract class LambdaHandler<TReq, TRes> : BaseLambdaHandler<TReq, TRes>
    {
        private IConfigurationService _configService;

        public LambdaHandler() : base()
        {
            EpsagonUtils.RegisterServices();
            _configService = EpsagonUtils.GetService<IConfigurationService>();
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
            var attrs = GetType().GetMethod(nameof(this.HandlerFunction)).GetCustomAttributes(typeof(EpsagonAttribute), false);
            if (attrs.Length > 0)
            {
                var epsagonAttr = (EpsagonAttribute)attrs[0];
                var attrConfig = _configService.FromAttribute(epsagonAttr);
                _configService.SetConfig(attrConfig);
            }

            var config = _configService.GetConfig();
            context.Logger.Log($"Appname: {config.AppName}");

            return this.HandlerFunction(input, context);
        }
    }
}
