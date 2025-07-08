using Microsoft.AspNetCore.Mvc;

namespace Csharp_periode_4_Groep_24_2025.Models.Interfaces
{
    public interface IDayNightCycle
    {
        public Task<IActionResult> Sunset();
        public Task<IActionResult> Sunrise();
    }
}
