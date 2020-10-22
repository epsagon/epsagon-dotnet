using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Epsagon.Dotnet.Lambda;
using Epsagon.Dotnet.Instrumentation;
using Epsagon.Dotnet.Instrumentation.EFCore;

namespace ef_core_trace
{
    public class Person
    {
        public int Id { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }
    }

    public class AppContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=app.db").UseEpsagon();
        }
    }

    class Program
    {
        static Program()
        {
            EpsagonBootstrap.Bootstrap();
        }

        static void Main() => EpsagonGeneralHandler.Handle(() =>
        {
            var rand = new Random();
            var person = new Person
            {
                Id = rand.Next(),
                Name = "test",
                Age = rand.Next()
            };

            using var db = new AppContext();
            db.Add(person);
            db.SaveChanges();

            var people = db.People.Where(p => p.Name == "test").ToList();
            Console.WriteLine($"people count: {people.Count}");
        });
    }
}
