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

        public void CheckAnimalConstraints(List<string> list, Enclosure? enclosure)
        {
            bool misconfiguration = false;
            if (EnclosureId != null)
            {
                if (SecurityRequirement > enclosure.Security)
                {
                    list.Add($"{enclosure.Name} does not meet the security requirement to house {Name}.");
                    misconfiguration = true;
                }
                foreach (var animal in enclosure.Animals)
                {
                    if (animal.Id != Id)
                    {
                        if (Prey == animal.Species || Species == animal.Prey)
                        {
                            list.Add($"{Name} and {animal.Name} shouldn't live in the same enclosure.");
                            misconfiguration = true;
                            break;
                        }
                        if (SpaceRequirement < enclosure.Size)
                        {
                            list.Add($"{enclosure.Name} is not big enough for {Name} to live in.");
                            misconfiguration = true;
                        }
                    }
                }
            }
            else
            {
                list.Add($"{Name} has not yet been assigned to an enclosure.");
                misconfiguration = true;
            }
            if (CategoryId == null)
            {
                list.Add($"{Name} has no category assigned to them yet");
                misconfiguration = true;
            }
            if (misconfiguration == false)
            {
                list.Add($"{Name} has no configuration mistakes.");
            }
        }
    }
}
