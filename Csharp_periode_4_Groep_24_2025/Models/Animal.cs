using Csharp_periode_4_Groep_24_2025.Data.Enum;

namespace Csharp_periode_4_Groep_24_2025.Models
{
    public class Animal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Species { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; } = null!;
        public Size SizeClass { get; set; }
        public DietaryClass Diet {  get; set; }
        public ActivityPattern Activity { get; set; }
        public string Prey { get; set; }
        public int? EnclosureId { get; set; }
        public Enclosure? Enclosure { get; set; } = null!;
        public double SpaceRequirement { get; set; }
        public SecurityLevel SecurityRequirement { get; set; }
    }
}
