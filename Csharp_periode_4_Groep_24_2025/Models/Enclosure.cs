using Csharp_periode_4_Groep_24_2025.Data.Enum;

namespace Csharp_periode_4_Groep_24_2025.Models
{
    public class Enclosure
    {
        public int Id {  get; set; }
        public string Name { get; set; }
        public ICollection<Animal> Animals { get; } = new List<Animal>();
        public Climate ClimateClass{ get; set; }
        public HabitatTypes HabitatType { get; set; }
        public SecurityLevel Security {  get; set; }
        public double Size { get; set; }
    }
}
