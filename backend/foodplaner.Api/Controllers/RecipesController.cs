using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/recipes")]
public class RecipesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllRecipes()
    {
        var recipes = await db.Recipes.ToListAsync();
        return Ok(recipes);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRecipeById(int id)
    {
        Recipe? recipe = await db.Recipes.FindAsync(id);
        if (recipe != null)
        {
            return Ok(recipe);
        }
        else
        {
            return NotFound();
        }
    }
    [HttpPost]
    public async Task<IActionResult> AddRecipe(Recipe recipe)
    {
        if (!recipe.IsValid())
        {
            return BadRequest("Invalid Input.");
        }
        db.Recipes.Add(recipe);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetRecipeById), new { id = recipe.Id }, recipe);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipe(int id)
    {
        Recipe? recipeToDelete = await db.Recipes.FindAsync(id);
        if (recipeToDelete != null)
        {
            db.Recipes.Remove(recipeToDelete);
            await db.SaveChangesAsync();
            return NoContent();
        }
        else
        {
            return NotFound();
        }
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> ChangeRecipe(int id, Recipe recipe)
    {
        if (!recipe.IsValid())
        {
            return BadRequest("Invalid Input.");
        }
        Recipe? recipeToChange = await db.Recipes.FindAsync(id);
        if (recipeToChange != null)
        {
            recipeToChange.Name = recipe.Name;
            recipeToChange.Instructions = recipe.Instructions;
            recipeToChange.Portions = recipe.Portions;
            await db.SaveChangesAsync();
            return Ok(recipeToChange);
        }
        else
        {
            return NotFound();
        }
    }
}