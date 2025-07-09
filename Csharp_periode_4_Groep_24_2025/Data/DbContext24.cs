using Csharp_periode_4_Groep_24_2025.Models;
using Csharp_periode_4_Groep_24_2025.Data.Enum;
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

            modelBuilder.Entity<Enclosure>().HasData(
                new Enclosure { Id = 1, Name = "Sigma", ClimateClass = Climate.Arctic, HabitatType = HabitatTypes.Aquatic, Security = SecurityLevel.High, Size = 320},
                new Enclosure { Id = 2, Name = "Delta", ClimateClass = Climate.Tropical, HabitatType = HabitatTypes.Desert, Security = SecurityLevel.Low, Size = 200 },
                new Enclosure { Id = 3, Name = "Epsilon", ClimateClass = Climate.Temperate, HabitatType = HabitatTypes.Grassland, Security = SecurityLevel.Low, Size = 240 },
                new Enclosure { Id = 4, Name = "Zeta", ClimateClass = Climate.Arctic, HabitatType = HabitatTypes.Grassland, Security = SecurityLevel.Low, Size = 240 },
                new Enclosure { Id = 5, Name = "Gamma", ClimateClass = Climate.None, HabitatType = HabitatTypes.Grassland, Security = SecurityLevel.Low, Size = 320 }
                );
            
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Fish"},
                new Category { Id = 2, Name = "Bird" },
                new Category { Id = 3, Name = "Mammal" }
                );

            modelBuilder.Entity<Animal>().HasData(
                new Animal { Id = 1, Name = "Bear", Prey = "Fish", Diet = DietaryClass.Omnivore, Activity = ActivityPattern.Diurnal, SizeClass = Size.Large, Species = "Bear", SecurityRequirement = SecurityLevel.Medium, SpaceRequirement = 300},
                new Animal { Id = 2, Name = "Eagle", Prey = "Fish", Diet = DietaryClass.Piscivore, Activity = ActivityPattern.Diurnal, SizeClass = Size.Medium, Species = "Bird", SecurityRequirement = SecurityLevel.Low, SpaceRequirement = 50 },
                new Animal { Id = 3, Name = "Salmon", Prey = "None", Diet = DietaryClass.Herbivore, Activity = ActivityPattern.Cathemeral, SizeClass = Size.Small, Species = "Fish", SecurityRequirement = SecurityLevel.Low, SpaceRequirement = 120 },
                new Animal { Id = 4, Name = "Wolf", Prey = "Deer", Diet = DietaryClass.Carnivore, Activity = ActivityPattern.Diurnal, SizeClass = Size.Large, Species = "Catlike", SecurityRequirement = SecurityLevel.Medium, SpaceRequirement = 300 },
                new Animal { Id = 5, Name = "Owl", Prey = "Fish", Diet = DietaryClass.Piscivore, Activity = ActivityPattern.Nocturnal, SizeClass = Size.Medium, Species = "Bird", SecurityRequirement = SecurityLevel.Low, SpaceRequirement = 200 }
                );
        }

        public DbSet<Animal> Animal { get; set; } = default!;
        public DbSet<Category> Category { get; set; } = default!;
        public DbSet<Enclosure> Enclosure { get; set; } = default!;
    }
}
