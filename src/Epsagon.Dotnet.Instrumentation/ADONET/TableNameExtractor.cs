using System;
using System.Linq;
using System.Collections.Generic;

namespace Epsagon.Dotnet.Instrumentation.ADONET
{
    public static class TableNameExtractor
    {
        public static Dictionary<string, string[]> _indicators = new Dictionary<string, string[]> {
            { "select", new [] {"from"} },
            { "insert", new [] {"into"} },
            { "update", new [] {"update"} },
            { "create", new [] {"exists", "table"}},
            { "alter", new [] {"table"}},
            { "drop", new [] {"exists", "table"}}
        };

        public static string ExtractTableName(string statement)
        {
            try
            {
                var lower = statement.ToLower();
                var words = lower.Split(null as char[], StringSplitOptions.RemoveEmptyEntries).ToList();
                var command = words.First();
                var indicators = _indicators[command];

                foreach (var indicator in indicators)
                {
                    if (words.Contains(indicator))
                        return words.SkipWhile(w => w != indicator).Skip(1).First();
                }
            }
            catch { }
            return "";
        }
    }
}
