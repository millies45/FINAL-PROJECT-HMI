using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrecisionFullCoilHMI.Data;
using PrecisionFullCoilHMI.Models;

namespace PrecisionFullCoilHMI.Controllers;
public class RecipeController : Controller
{
    private readonly ApplicationDbContext _context;

    public RecipeController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Recipe
    public async Task<IActionResult> Index()
    {
        var recipes = await _context.Recipes.Include(r => r.Jobs).ToListAsync();
        return View(recipes);
    }

    // GET: Recipe/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var recipe = await _context.Recipes
            .Include(r => r.Jobs)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null)
        {
            return NotFound();
        }

        return View(recipe);
    }

    // GET: Recipe/CreateAndEdit
    public async Task<IActionResult> CreateAndEdit()
    {
        var newRecipe = new Recipe
        {
            Name = "New Recipe",
            Description = "Default Description",
            UserId = "TestUser", // Assuming the user is logged in
            CreatedDate = DateTime.UtcNow,
            LastUpdateDate = DateTime.UtcNow,
            NumberOfJobs = 1
        };

        // Create a default job with JobNumber set to 1
        newRecipe.Jobs.Add(new Job
        {
            JobNumber = 1,
            Quantity = 0,
            SideA = 0,
            SideB = 0,
            DuctType = 0,
            Lock = 0,
            Connector = 0,
            Cleats = 0,
            CleatEdges = 0,
            SideAHoles = 0,
            SideBHoles = 0,
            HoleDie = 0,
            HoleSize = 0,
            Bead = 0,
            Insulation = 0,
            PinSpacing = 0,
            Sealant = 0,
            Gauge = 0,
            LastUpdateDate = DateTime.UtcNow
        });

        _context.Recipes.Add(newRecipe);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = newRecipe.Id });
    }

    // POST: Recipe/Update
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, [Bind("Id,Name,Description,NumberOfJobs")] Recipe recipe)
    {
        if (id != recipe.Id)
        {
            return NotFound();
        }

        var existingRecipe = await _context.Recipes
            .Include(r => r.Jobs)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (existingRecipe == null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            existingRecipe.Name = recipe.Name;
            existingRecipe.Description = recipe.Description;
            existingRecipe.LastUpdateDate = DateTime.UtcNow;

            // Adjust the number of jobs
            if (existingRecipe.NumberOfJobs < recipe.NumberOfJobs)
            {
                // Add more jobs
                for (int i = existingRecipe.NumberOfJobs + 1; i <= recipe.NumberOfJobs; i++)
                {
                    existingRecipe.Jobs.Add(new Job
                    {
                        JobNumber = (short)i,
                        RecipeId = existingRecipe.Id,
                        Quantity = 0, // Default values
                        SideA = 0,
                        SideB = 0,
                        DuctType = 0,
                        Lock = 0,
                        Connector = 0,
                        Cleats = 0,
                        CleatEdges = 0,
                        SideAHoles = 0,
                        SideBHoles = 0,
                        HoleDie = 0,
                        HoleSize = 0,
                        Bead = 0,
                        Insulation = 0,
                        PinSpacing = 0,
                        Sealant = 0,
                        Gauge = 0,
                        LastUpdateDate = DateTime.UtcNow
                    });
                }
            }
            else if (existingRecipe.NumberOfJobs > recipe.NumberOfJobs)
            {
                // Remove excess jobs
                var jobsToRemove = existingRecipe.Jobs
                    .Where(j => j.JobNumber > recipe.NumberOfJobs)
                    .ToList();

                _context.Jobs.RemoveRange(jobsToRemove);
            }

            existingRecipe.NumberOfJobs = recipe.NumberOfJobs;

            _context.Update(existingRecipe);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View("Details", existingRecipe);
    }


    [HttpPost]
    public async Task<IActionResult> UpdateJobCount(int recipeId, int newJobCount)
    {
        var recipe = await _context.Recipes.Include(r => r.Jobs).FirstOrDefaultAsync(r => r.Id == recipeId);
        if (recipe == null)
        {
            return Json(new { success = false, message = "Recipe not found." });
        }

        int currentJobCount = recipe.Jobs.Count;

        if (newJobCount > currentJobCount)
        {
            // Add new jobs
            for (int i = currentJobCount + 1; i <= newJobCount; i++)
            {
                recipe.Jobs.Add(new Job
                {
                    JobNumber = (short)i, // Assuming JobNumber is short
                    Quantity = 1, // Default values
                    SideA = 1,
                    SideB = 1,
                    DuctType = 1,
                    Lock = 1,
                    Connector = 1,
                    Cleats = 1,
                    CleatEdges = 1,
                    SideAHoles = 1,
                    SideBHoles = 1,
                    HoleDie = 1,
                    HoleSize = 1,
                    Bead = 1,
                    Insulation = 1,
                    PinSpacing = 1,
                    Sealant = 1,
                    Gauge = 1,
                    LastUpdateDate = DateTime.UtcNow,
                    RecipeId = recipe.Id
                });
            }
        }
        else if (newJobCount < currentJobCount)
        {
            // Remove excess jobs
            var jobsToRemove = recipe.Jobs.OrderByDescending(j => j.JobNumber).Take(currentJobCount - newJobCount).ToList();
            foreach (var job in jobsToRemove)
            {
                _context.Jobs.Remove(job);
            }
        }

        recipe.NumberOfJobs = (short)newJobCount;
        recipe.LastUpdateDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }
}
