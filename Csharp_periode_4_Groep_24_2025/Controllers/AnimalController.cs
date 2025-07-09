using Csharp_periode_4_Groep_24_2025.Data;
using Csharp_periode_4_Groep_24_2025.Data.Enum;
using Csharp_periode_4_Groep_24_2025.Models;
using Csharp_periode_4_Groep_24_2025.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Csharp_periode_4_Groep_24_2025.Controllers
{
    public class AnimalController : Controller, IDayNightCycle, ICheckConstraints
    {
        private readonly DbContext24 _context;
        private readonly ILogger<AnimalController> _logger;

        public AnimalController(DbContext24 context, ILogger<AnimalController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("api/[controller]")]
        public async Task<IActionResult> Index(string searchString)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                var result = _context.Animal.Include(z => z.Enclosure)
                .Include(z => z.Category).Where(x => x.Name.Contains(searchString)
                                                || x.Species.Contains(searchString)
                                                || x.Category.Name.Contains(searchString)
                                                || x.Enclosure.Name.Contains(searchString)
                                                || x.Diet.ToString().Contains(searchString)
                                                || x.Activity.ToString().Contains(searchString))
                ;
                return View(await result.ToListAsync());
            }
            else
            {
                var result = _context.Animal.Include(z => z.Enclosure)
                .Include(z => z.Category);
                return View(await result.ToListAsync());
            }
        }

        [HttpGet("api/[controller]/create")]
        public IActionResult Create()
        {
            PopulateCategoryDropDownList();
            PopulateEnclosureDropDownList();
            return View();
        }

        [HttpPost("api/[controller]/create")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Create(Animal animal)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(animal);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Failed to save changes.");
            }
            return View(animal);
        }

        [HttpGet("api/[controller]/{id:int}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animal
                 .Include(s => s.Enclosure)
                 .Include(e => e.Category)
                 .AsNoTracking()
                 .FirstOrDefaultAsync(m => m.Id == id);

            if (animal == null)
            {
                return NotFound();
            }

            return View(animal);
        }

        [HttpGet("api/[controller]/{id:int}/edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animal
                 .Include(s => s.Enclosure)
                 .Include(e => e.Category)
                 .AsNoTracking()
                 .FirstOrDefaultAsync(m => m.Id == id);

            if (animal == null)
            {
                return NotFound();
            }
            PopulateCategoryDropDownList();
            PopulateEnclosureDropDownList();
            return View(animal);
        }

        [HttpPost("api/[controller]/{id:int}/edit")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Edit(int? id, [Bind("Name", "Species", "CategoryId", "SizeClass", "Diet", "Activity", "Prey", "EnclosureId", "SpaceRequirement", "SecurityRequirement")] Animal animal)
        {
            if (id == null)
            {
                return NotFound();
            }
            var target = await _context.Animal.FirstOrDefaultAsync(z => z.Id == id);
            if (target == null)
            {
                return NotFound();
            }
            if (await TryUpdateModelAsync<Animal>(target, "", z => z.Name, z => z.Species, z => z.CategoryId, z => z.SizeClass, z => z.Diet, z => z.Activity, z => z.Prey, z => z.EnclosureId, z => z.SpaceRequirement, z => z.SecurityRequirement))
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
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var target = await _context.Animal
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (target == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(target);
        }

        [HttpPost("api/[controller]/{id:int}/delete")]
        public async Task<IActionResult> ExecuteDeletion(int? id, bool? saveChangesError = false)
        {
            var target = await _context.Animal.FindAsync(id);
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }
            try
            {
                _context.Animal.Remove(target);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["Errormessage"] = "Failed to delete";
            }

            return RedirectToAction(nameof(Index));
        }

        private void PopulateCategoryDropDownList(object? selectedCategory = null)
        {
            var categoryQuery = _context.Category
                                    .OrderBy(d => d.Name)
                                    .AsNoTracking()
                                    .ToList();

            ViewBag.CategoryId = new SelectList(categoryQuery, "Id", "Name", selectedCategory);
        }

        private void PopulateEnclosureDropDownList(object? selectedEnclosure = null)
        {
            var enclosureQuery = _context.Enclosure
                                    .OrderBy(d => d.Name)
                                    .AsNoTracking()
                                    .ToList();

            ViewBag.EnclosureId = new SelectList(enclosureQuery, "Id", "Name", selectedEnclosure);
        }

        [HttpGet("api/[controller]/{id:int}/sunset")]
        public async Task<IActionResult> Sunset(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var target = await _context.Animal
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (target == null)
            {
                return NotFound();
            }
            if (target.Activity == ActivityPattern.Diurnal)
            {
                ViewBag.Status = $"{target.Name} is currently asleep, night night {target.Name}";
            }
            else if (target.Activity == ActivityPattern.Nocturnal)
            {
                ViewBag.Status = $"{target.Name} has just awakened, good luck {target.Name}";
            }
            else if (target.Activity == ActivityPattern.Cathemeral)
            {
                ViewBag.Status = $"{target.Name} is already awake, {target.Name} doesn't need sleep";
            }
            else
            {
                ViewBag.Status = $"{target.Name}'s activity pattern has not been set, please change this by editing the day/night behaviour";
            }
            return View(target);
        }

        [HttpGet("api/[controller]/{id:int}/sunrise")]
        public async Task<IActionResult> Sunrise(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var target = await _context.Animal
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (target == null)
            {
                return NotFound();
            }
            if (target.Activity == ActivityPattern.Nocturnal)
            {
                ViewBag.Status = $"{target.Name} is currently asleep, sleep well {target.Name}";
            }
            else if (target.Activity == ActivityPattern.Diurnal)
            {
                ViewBag.Status = $"{target.Name} has just awakened, good luck {target.Name}";
            }
            else if (target.Activity == ActivityPattern.Cathemeral)
            {
                ViewBag.Status = $"{target.Name} is already awake, {target.Name} doesn't need sleep";
            }
            else
            {
                ViewBag.Status = $"{target.Name} activity pattern has not been set, please change this by editing the day/night behaviour";
            }
            return View(target);
        }

        [HttpGet("api/[controller]/{id:int}/feedingtime")]
        public async Task<IActionResult> FeedingTime(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var target = await _context.Animal
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (target == null)
            {
                return NotFound();
            }
            if (target.Diet == DietaryClass.Herbivore)
            {
                ViewBag.Foodcommentary = $"{target.Name} is a herbivore, they like to eat plants";
            }
            else if (target.Diet != null && target.Prey != null && target.Diet != DietaryClass.None)
            {
                ViewBag.Foodcommentary = $"{target.Name} is a {target.Diet}, their prey is {target.Prey}";
            }
            else if (target.Prey != null)
            {
                ViewBag.Foodcommentary = $"{target.Name}'s diet is not classified, though they have a favorite prey, {target.Prey}";
            }
            else if (target.Diet != null && target.Diet != DietaryClass.None)
            {
                ViewBag.Foodcommentary = $"{target.Name} is a {target.Diet}, but they don't have a prey";
            }
            else
            {
                ViewBag.Foodcommentary = $"It is unknown what {target.Name} likes to eat, there is no diet classification nor favorite prey set";
            }
            return View(target);
        }

        [HttpGet("api/[controller]/{id:int}/check")]
        public async Task<IActionResult> CheckConstraints(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var target = await _context.Animal
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (target == null)
            {
                return NotFound();
            }
            var targetEnclosure = await _context.Enclosure
                .AsNoTracking()
                .FirstOrDefaultAsync(z => z.Id == target.EnclosureId);
            List<string> commentary = new List<string>();
            if (targetEnclosure != null)
            {
                target.CheckAnimalConstraints(commentary, targetEnclosure);
            }
            else
            {
                target.CheckAnimalConstraints(commentary, null);
            }
            ViewBag.Id = id;
            return View(commentary);
        }
    }
}
