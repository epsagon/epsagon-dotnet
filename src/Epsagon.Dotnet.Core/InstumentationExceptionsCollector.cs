using System;
using System.Collections.Generic;

namespace Epsagon.Dotnet.Core {
    public static class InstumentationExceptionsCollector {
        public static List<Exception> Exceptions;

        static InstumentationExceptionsCollector() {
            Exceptions = new List<Exception>();
        }
    }
}
