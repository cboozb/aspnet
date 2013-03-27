using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ODataActionsSample.Models
{
    public class MoviesContext : DbContext
    {
        static MoviesContext()
        {
            Database.SetInitializer(new MoviesInitializer());
        }

        public MoviesContext()
            : base("name=DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Set the TimeStamp property to be an optimistic concurrency token.
            // EF will use this to detect concurrency conflicts.

            modelBuilder.Entity<Movie>()
                .Property(m => m.TimeStamp)
                .IsConcurrencyToken()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Movie> Movies { get; set; }
    }

    public class MoviesInitializer : DropCreateDatabaseIfModelChanges<MoviesContext>
    {
        protected override void Seed(MoviesContext context)
        {
            List<Movie> movies = new List<Movie>()
            {
                new Movie() { Title = "Maximum Payback" },
                new Movie() { Title = "Inferno of Retribution" },
                new Movie() { Title = "Fatal Vengeance 2" },
                new Movie() { Title = "Sudden Danger" },
                new Movie() { Title = "Deadly Honor IV" }
            };
            movies.ForEach(m => context.Movies.Add(m));
            context.SaveChanges();
        }
    }
}