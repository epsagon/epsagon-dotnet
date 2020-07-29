
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.Builder {
    public static class EpsagonMiddlewareExtensions
    {
        public static IApplicationBuilder UseEpsagon(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EpsagonMiddleware.EpsagonMiddleware>();
        }
    }
}