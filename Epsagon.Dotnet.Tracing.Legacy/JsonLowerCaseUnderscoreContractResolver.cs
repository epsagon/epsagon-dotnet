using System.Text.RegularExpressions;
using Newtonsoft.Json.Serialization;

namespace Epsagon.Dotnet.Tracing.Legacy
{
    public class JsonLowerCaseUnderscoreContractResolver : DefaultContractResolver
    {
        private Regex regex = new Regex("(?!(^[A-Z]))([A-Z])");

        protected override string ResolvePropertyName(string propertyName)
        {
            return regex.Replace(propertyName, "_$2").ToLower();
        }
    }

}
