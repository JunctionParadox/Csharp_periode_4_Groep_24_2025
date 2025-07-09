using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Csharp_periode_4_Groep_24_2025.Models;
using Csharp_periode_4_Groep_24_2025.Data;
using Csharp_periode_4_Groep_24_2025.Data.Enum;
using Microsoft.EntityFrameworkCore;
using Csharp_periode_4_Groep_24_2025.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Csharp_periode_4_Groep_24_2025.Controllers
{
    public class EnclosureController : Controller, IDayNightCycle, ICheckConstraints
    {
        private readonly DbContext24 _context;
        private readonly ILogger<EnclosureController> _logger;

        public EnclosureController(DbContext24 context, ILogger<EnclosureController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("api/[controller]")]
        public async Task<IActionResult> Index()
        {
            var result = _context.Enclosure.Include(z => z.Animals);
            return View(await result.ToListAsync());
        }

        [HttpGet("/api/[controller]/create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("/api/[controller]/create")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Create(Enclosure enclosure)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(enclosure);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Failed to save changes.");
            }
            return View(enclosure);
        }

        [HttpGet("api/[controller]/{id:int}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enclosure = await _context.Enclosure
                    .Include(e => e.Animals)
                    .FirstOrDefaultAsync(m => m.Id == id);

            if (enclosure == null)
            {
                return NotFound();
            }

            return View(enclosure);
        }

        [HttpGet("api/[controller]/{id:int}/edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enclosure = await _context.Enclosure
                 .AsNoTracking()
                 .FirstOrDefaultAsync(m => m.Id == id);

            if (enclosure == null)
            {
                return NotFound();
            }
            PopulateAnimalDropDownList();
            return View(enclosure);
        }

        [HttpPost("api/[controller]/{id:int}/edit")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Edit(int? id, [Bind("Name", "ClimateClass", "HabitatType", "Security", "Size")] Enclosure enclosure, int? animalId)
        {
            if (id == null)
            {
                return NotFound();
            }
            var target = await _context.Enclosure.FirstOrDefaultAsync(z => z.Id == id);
            if (target == null)
            {
                return NotFound();
            }
            var animal = await _context.Animal.FindAsync(animalId);
            if (animal == null)
            {
                return NotFound("Animal not found");
            }
            if (!target.Animals.Contains(animal))
            {
                target.Animals.Add(animal);
            }
            if (await TryUpdateModelAsync<Enclosure>(target, "", z => z.Name, z => z.ClimateClass, z => z.HabitatType, z => z.Security, z => z.Size))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id });
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Failed to update");
                }
            }
            return RedirectToAction("Details", new { id });

        }

        [HttpGet("api/[controller]/{id:int}/delete")]
        public async Task<IActionResult> Delete (int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }
            var target = await _context.Enclosure
                .AsNoTracking()
                .FirstOrDefaultAsync(z => z.Id == id);
            if (target == null)
            {
                return NotFound();
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Please try again";
            }

            return View(target);
        }

        [HttpPost("api/[controller]/{id:int}/delete")]
        public async Task<IActionResult> ExecuteDeletion (int? id, bool? saveChangesError = false)
        {
            var target = await _context.Enclosure.FindAsync(id);
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }
            try
            {
                _context.Enclosure.Remove(target);
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["Errormessage"] = "Failed to delete";
            }

            return RedirectToAction(nameof(Index));
        }

        private void PopulateAnimalDropDownList(object? selectedAnimal = null)
        {
            var animalQuery = _context.Animal
                                    .OrderBy(d => d.Name)
                                    .AsNoTracking()
                                    .ToList();

            ViewBag.AnimalId = new SelectList(animalQuery, "Id", "Name", selectedAnimal);
        }

        [HttpGet("api/[controller]/{id:int}/sunset")]
        public async Task<IActionResult> Sunset(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var target = await _context.Enclosure
                .AsNoTracking()
                .Include(e => e.Animals)
                .FirstOrDefaultAsync(z => z.Id == id);
            if (target == null)
            {
                return NotFound();
            }
            List<string> animalList = new List<string>();
            foreach(var animal in target.Animals)
            {
                if (animal.Activity == ActivityPattern.Diurnal)
                {
                    animalList.Add($"{animal.Name} is going to sleep.");
                }
                else if (animal.Activity == ActivityPattern.Nocturnal)
                {
                    animalList.Add($"{animal.Name} is going to awake from their slumber.");
                }
                else if (animal.Activity == ActivityPattern.Cathemeral)
                {
                    animalList.Add($"{animal.Name} is already awake.");
                }
                else
                {
                    animalList.Add($"No info could be found about the activity pattern of {animal.Name}, consider adding some in editing panel.");
                }
            }
            return View(animalList);
        }

        [HttpGet("api/[controller]/{id:int}/sunrise")]
        public async Task<IActionResult> Sunrise(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var target = await _context.Enclosure
                .AsNoTracking()
                .Include(e => e.Animals)
                .FirstOrDefaultAsync(z => z.Id == id);
            if (target == null)
            {
                return NotFound();
            }
            List<string> animalList = new List<string>();
            foreach (var animal in target.Animals)
            {
                if (animal.Activity == ActivityPattern.Nocturnal)
                {
                    animalList.Add($"{animal.Name} is going to sleep.");
                }
                else if (animal.Activity == ActivityPattern.Diurnal)
                {
                    animalList.Add($"{animal.Name} is going to awake from their slumber.");
                }
                else if (animal.Activity == ActivityPattern.Cathemeral)
                {
                    animalList.Add($"{animal.Name} is already awake.");
                }
                else
                {
                    animalList.Add($"No info could be found about the activity pattern of {animal.Name}, consider adding some in editing panel.");
                }
            }
            return View(animalList);
        }

        //Het idee is dat door Insert(0, string) te gebruiken, alle non-vegetarische dieren boven de vegetarische dieren worden gerangschikt
        [HttpGet("api/[controller]/{id:int}/feedingtime")]
        public async Task<IActionResult> FeedingTime(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var target = await _context.Enclosure
                .AsNoTracking()
                .Include(e => e.Animals)
                .FirstOrDefaultAsync(z => z.Id == id);
            if (target == null)
            {
                return NotFound();
            }
            List<string> animalList = new List<string>();
            foreach (var animal in target.Animals)
            {
                if (animal.Diet == DietaryClass.Herbivore)
                {
                    animalList.Add($"{animal.Name} is a herbivore, they like to eat plants.");
                }
                else if (animal.Diet != DietaryClass.None && animal.Prey != null)
                {
                    animalList.Insert(0, $"{animal.Name} is a {animal.Diet}, they like to eat {animal.Prey}.");
                }
                else if (animal.Diet != DietaryClass.None)
                {
                    animalList.Insert(0, $"{animal.Name} is a {animal.Diet}, but we do not know their prey.");
                }
                else if (animal.Prey != null)
                {
                    animalList.Add($"{animal.Name} likes to eat {animal.Prey}.");
                }
                else
                {
                    animalList.Add($"{animal.Name} has no known diet or prey, please consider adding more info.");
                }
            }
            return View(animalList);
        }

        [HttpGet("api/[controller]/{id:int}/check")]
        public async Task<IActionResult> CheckConstraints(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var target = await _context.Enclosure
                .AsNoTracking()
                .Include(e => e.Animals)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (target == null)
            {
                return NotFound();
            }
            List<string> commentary = new List<string>();
            foreach(var animal in target.Animals)
            {
                animal.CheckAnimalConstraints(commentary, target);
            }
            ViewBag.Id = id;
            return View(commentary);
        }
    }
}
