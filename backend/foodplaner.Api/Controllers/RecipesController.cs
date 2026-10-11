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
    public async Task<IActionResult> AddRecipe(CreateRecipeRequest request)
    {
        var error = CheckRecipe(request);
        if(error != null)
        {
            return BadRequest(error);
        }
        Recipe recipe = new Recipe
        {
            Name = request.Name,
            Portions = request.Portions,
            Instructions = request.Instructions
        };
        var ingredients = request.ingredients ?? [];

        //Check Request in DB
        foreach(var item in ingredients)
        {
            var name = item.Name.Trim();
            var ingredient = await db.Ingredients
                .FirstOrDefaultAsync(i => i.Name.ToLower() == name.ToLower());
            if(ingredient == null)
            {
                if(item.unit == null)
                {
                    return BadRequest("The unit is missing.");
                }
                ingredient = new Ingredient {Name = name, IngredientUnit = item.unit.Value };
            }
            recipe.RecipeIngredients.Add(new RecipeIngredient
            {
                Ingredient = ingredient,
                Amount = item.Amount
            });
        }
        db.Recipes.Add(recipe);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetRecipeById), new { id = recipe.Id }, new {recipe.Id});
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
    public async Task<IActionResult> ChangeRecipe(int id, CreateRecipeRequest request)
    {
        var error = CheckRecipe(request);
        if(error != null)
        {
            return BadRequest(error);
        }
        var ingredients = request.ingredients ?? [];
        var names = ingredients.Select(i => i.Name.Trim().ToLower()).ToList();

        //Load Recipe with Ingredients
        Recipe? recipeToChange = await db.Recipes
            .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
            .FirstOrDefaultAsync(r => r.Id == id);
        
        if(recipeToChange == null)
        {
            return NotFound();
        }
        recipeToChange.Name = request.Name;
        recipeToChange.Portions = request.Portions;
        recipeToChange.Instructions = request.Instructions;

        //Remove Ingredients
        var toRemove = recipeToChange.RecipeIngredients
            .Where(ri => !names.Contains(ri.Ingredient.Name.ToLower()))
            .ToList();

        foreach(var ri in toRemove)
        {
            db.RecipeIngredients.Remove(ri);
        }
        //Change Amounts and add new Ingredients
        foreach(var item in ingredients)
        {
            var name = item.Name.Trim();
            var existingRow = recipeToChange.RecipeIngredients
                .FirstOrDefault(ri => ri.Ingredient.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if(existingRow != null)
            {
                existingRow.Amount = item.Amount;
                continue;
            }

            var ingredient = await FindOrCreateIngredient(name, item.unit);
            if(ingredient == null)
            {
                return BadRequest("Unit is missing.");
            }
            recipeToChange.RecipeIngredients.Add(new RecipeIngredient
            {
                Ingredient = ingredient,
                Amount = item.Amount
            });
        }
        await db.SaveChangesAsync();
        return NoContent();
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
    private async Task<Ingredient> FindOrCreateIngredient(string name, Unit? unit)
    {
        var ingredient = await db.Ingredients.FirstOrDefaultAsync(i => i.Name.ToLower() == name.ToLower());
        if(ingredient != null)
        {
            return ingredient;
        }
        if(unit == null)
        {
            return null;
        }
        return new Ingredient {Name = name, IngredientUnit = unit.Value};
    }

    private string? CheckRecipe(CreateRecipeRequest request)
    {
        var recipe = new Recipe
        {
            Name = request.Name,
            Portions = request.Portions,
            Instructions = request.Instructions
        };
        if (!recipe.IsValid())
        {
            return "Invalid input.";
        }

        var ingredients = request.ingredients ?? [];

        foreach (var item in ingredients)
        {
            if(string.IsNullOrWhiteSpace(item.Name) || item.Amount <= 0)
            {
                return "Invalid input";
            }
        }

        var names = ingredients.Select(i => i.Name.Trim().ToLower()).ToList();
        if(names.Count != names.Distinct().Count())
        {
            return "Ingredient appears multiple times.";
        }
        return null;
    }
}
