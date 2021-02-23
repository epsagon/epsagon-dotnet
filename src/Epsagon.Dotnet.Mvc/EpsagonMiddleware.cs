using System;
using System.Threading.Tasks;

using Epsagon.Dotnet.Mvc;

using Microsoft.AspNetCore.Http;

using OpenTracing.Util;

namespace EpsagonMiddleware {
    public class EpsagonMiddleware {
        private readonly RequestDelegate _next;

        public EpsagonMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context) {
            await EpsagonMvcHandler.Handle(async () => {
                await _next.Invoke(context);
            }, context);
        }
    }
}
