using System;
using Dapper;
using System.Data.SQLite;
using System.Linq;
using Epsagon.Dotnet.Instrumentation.ADONET;
using Epsagon.Dotnet.Lambda;
using Epsagon.Dotnet.Instrumentation;
using Npgsql;

namespace dapper_tracing
{
    class Person
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    class Program
    {

        static void SQLite()
        {
            var conn = new SQLiteConnection($"Data Source={Environment.CurrentDirectory}/SimpleDb.sqlite")
                .UseEpsagon();

            conn.Execute(@"CREATE TABLE IF NOT EXISTS people (
                    ID      INTEGER PRIMARY KEY,
                    Name    varchar(100) not null
                )");

            var rand = new Random();
            var person = new Person() { ID = rand.Next(), Name = "test" };

            var id = conn.Query<long>(
                @"INSERT INTO people ( ID, Name ) VALUES ( @ID, @Name );
                    SELECT last_insert_rowid()", person
                ).FirstOrDefault();

            var people = conn.Query<Person>("SELECT * FROM people").AsList();

            Console.WriteLine($"Inserted ID: {id}");
            Console.WriteLine($"Queried ${people.Count} people");

        }

        static void Postgres()
        {
            var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Database=postgres").UseEpsagon();
            conn.Open();

            conn.Execute(@"CREATE TABLE IF NOT EXISTS people (
                    ID      INTEGER PRIMARY KEY,
                    Name    varchar(100) not null
                )");

            var rand = new Random();
            var person = new Person() { ID = rand.Next(), Name = "test" };

            var id = conn.Query<long>(
                @"INSERT INTO people ( ID, Name ) VALUES ( @ID, @Name )",
                  person).FirstOrDefault();

            var people = conn.Query<Person>("SELECT * FROM people").AsList();

            Console.WriteLine($"Inserted ID: {id}");
            Console.WriteLine($"Queried ${people.Count} people");
        }

        static void Main()
        {
            EpsagonBootstrap.Bootstrap();
            EpsagonGeneralHandler.Handle(() => Postgres());
            // // Or:
            // EpsagonGeneralHandler.Handle(() => SQLite());
        }
    }
}
