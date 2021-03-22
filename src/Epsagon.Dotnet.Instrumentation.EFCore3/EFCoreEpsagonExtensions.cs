using Microsoft.EntityFrameworkCore;

namespace Epsagon.Dotnet.Instrumentation.EFCore3 {
    public static class EFCoreEpsagonExtensions {
        /// <summary>
        /// Adds Epsagon's command interceptor to the interceptors list
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder UseEpsagon(this DbContextOptionsBuilder options) {
            return options.AddInterceptors(new EpsagonCommandInterceptor());
        }
    }
}
