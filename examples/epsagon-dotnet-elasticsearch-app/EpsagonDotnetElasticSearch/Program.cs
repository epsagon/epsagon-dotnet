using System;
using System.Linq;
using Nest;
using Epsagon.Dotnet.Instrumentation.ElasticSearch;

namespace EpsagonDotnetElasticSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new ConnectionSettings()
                .DefaultIndex("people")
                .UseEpasgon();

            var client = new ElasticClient(settings);
            var results = client.Search<Person>(s => s
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Age)
                        .Query("25")
                    )
                )
            );

            System.Console.WriteLine(
                string.Join(
                    Environment.NewLine,
                    results.Documents.Select(p => $"[{p.Id}] {p.Name} ({p.Age})")
                )
            );
        }
    }
}
