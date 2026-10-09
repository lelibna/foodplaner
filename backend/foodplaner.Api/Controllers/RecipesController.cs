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
        Recipe? recipe = await db.Recipes
            .Include(r => r.RecipeIngredients)
            .ThenInclude(rI => rI.Ingredient)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (recipe == null)
        {
            return NotFound();
        }
        var recipetransfer = new RecipeDetailTransfer(
            recipe.Id,
            recipe.Name,
            recipe.Portions,
            recipe.Instructions,
            recipe.RecipeIngredients
                .Select(ri => new RecipeIngredientTransfer(
                    ri.IngredientId,
                    ri.Ingredient.Name,
                    ri.Amount,
                    ri.Ingredient.IngredientUnit))
                .ToList());
        return Ok(recipetransfer);
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
    [HttpPost("{recipeId}/ingredients")]
    public async Task<IActionResult> AddRecipeIngredient(int recipeId, AddRecipeIngredient recipeIngredient)
    {
        if(recipeIngredient.Amount <= 0)
        {
            return BadRequest("Amount is below 0");
        }        
        if (await db.RecipeIngredients.AnyAsync(r => r.RecipeId == recipeId && r.IngredientId == recipeIngredient.IngredientId))
        {
            return Conflict("Recipe ingredient already added.");
        }
        if (!await db.Recipes.AnyAsync(r => r.Id == recipeId))
        {
            return NotFound();
        }
        if (!await db.Ingredients.AnyAsync(i => i.Id == recipeIngredient.IngredientId))
        {
            return NotFound();
        }
        RecipeIngredient rI = new RecipeIngredient
        {
            RecipeId = recipeId,
            IngredientId = recipeIngredient.IngredientId,
            Amount = recipeIngredient.Amount
        };
        db.RecipeIngredients.Add(rI);
        await db.SaveChangesAsync();
        return NoContent();

    }
}