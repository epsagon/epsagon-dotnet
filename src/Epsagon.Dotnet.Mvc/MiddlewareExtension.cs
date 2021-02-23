
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Builder {
    public static class EpsagonMiddlewareExtensions {
        public static IApplicationBuilder UseEpsagon(
            this IApplicationBuilder builder) {
            return builder.UseMiddleware<EpsagonMiddleware.EpsagonMiddleware>();
        }
    }
}
