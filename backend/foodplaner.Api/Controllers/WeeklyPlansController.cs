using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/weeklyplans")]

public class WeeklyPlansController(AppDbContext db) : ControllerBase
{
    [HttpGet("{start}")]
    public async Task<IActionResult> GetWeeklyPlan(DateOnly start)
    {
        WeeklyPlan? weeklyPlan = await db.WeeklyPlans
            .Include(w => w.PlanEntries)
            .ThenInclude(pe => pe.Recipe)
            .FirstOrDefaultAsync(w => w.Start == start);
        if(weeklyPlan == null)
        {
            return Ok(new WeeklyPlanTransfer(start, []));
        }
        var weeklyPlanTransfer = new WeeklyPlanTransfer(
            weeklyPlan.Start,
            weeklyPlan.PlanEntries
                .OrderBy(pe => pe.Date)
                .ThenBy(pe => pe.Mealtime)
                .Select(pe => new PlanEntryTransfer(
                    pe.Id,
                    pe.Date,
                    pe.Mealtime,
                    pe.Portions,
                    pe.RecipeId,
                    pe.Recipe.Name))
                .ToList()
        );
        return Ok(weeklyPlanTransfer);
    }

    [HttpPost("{start}/entries")]
    public async Task<IActionResult> PostPlanEntry(DateOnly start, CreatePlanEntryRequest request)
    {
        if(!request.IsValidFor(start))
        {
            return BadRequest("Start date doesn't match Week start date.");
        }
        if(!await db.Recipes.AnyAsync(r => r.Id == request.RecipeId))
        {
            return NotFound("Recipe not found.");
        }

        WeeklyPlan? weeklyPlan = await db.WeeklyPlans
            .FirstOrDefaultAsync(w => w.Start == start);
        
        if(weeklyPlan == null)
        {
            weeklyPlan = new WeeklyPlan {Start = start};
            db.WeeklyPlans.Add(weeklyPlan);
        }

        var planEntry = new PlanEntry
        {
            Date = request.Date,
            Mealtime = request.Mealtime,
            Portions = request.Portions,
            RecipeId = request.RecipeId
        };

        weeklyPlan.PlanEntries.Add(planEntry);
        await db.SaveChangesAsync();

        return Created($"/api/weeklyplans/{start:yyyy-MM-dd}", new { planEntry.Id });
    }
    [HttpDelete("{start}/entries/{id}")]
    public async Task<IActionResult> DeletePlanEntry(DateOnly start, int id)
    {
        PlanEntry? planEntry = await db.PlanEntries
            .FirstOrDefaultAsync(pe => pe.Id == id && pe.WeeklyPlan.Start == start);
        
        if(planEntry == null)
        {
            return NotFound();
        }
        db.PlanEntries.Remove(planEntry);
        await db.SaveChangesAsync();
        return NoContent();
    }
}