namespace Csharp_periode_4_Groep_24_2025.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Animal> Animals { get; } = new List<Animal>();
    }
}
