using Csharp_periode_4_Groep_24_2025.Models;
using Microsoft.EntityFrameworkCore;

namespace Csharp_periode_4_Groep_24_2025.Data
{
    public class DbContext24 : DbContext
    {
        public DbContext24(DbContextOptions<DbContext24> options) : base(options) { }

        public DbSet<Animal> Animals;
        public DbSet<Category> Cateogries;
        public DbSet<Enclosure> Enclosures;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Animal>().ToTable("Animals");
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Enclosure>().ToTable("Enclosures");
        }

        public DbSet<Animal> Animal { get; set; } = default!;
        public DbSet<Category> Category { get; set; } = default!;
        public DbSet<Enclosure> Enclosure { get; set; } = default!;
    }
}
