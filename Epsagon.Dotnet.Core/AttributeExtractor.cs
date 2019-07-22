using System;
using System.Reflection;
namespace Epsagon.Dotnet.Core
{
    public class AttributeExtractor
    {
        public static T GetAttribute<T>() where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(T));
        }
    }
}
