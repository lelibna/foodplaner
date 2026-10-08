using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/ingredients")]
public class IngredientsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllIngredients()
    {
        var ingredients = await db.Ingredients.ToListAsync();
        return Ok(ingredients);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetIngredientById(int id)
    {
        Ingredient? ingredient = await db.Ingredients.FindAsync(id);
        if (ingredient != null)
        {
            return Ok(ingredient);
        }
        else
        {
            return NotFound();
        }
    }
    [HttpPost]
    public async Task<IActionResult> AddIngredient(Ingredient ingredient)
    {
        if (!ingredient.IsValid())
        {
            return BadRequest("Invalid Input.");
        }
        if (await db.Ingredients.AnyAsync(n => n.Name == ingredient.Name))
        {
            return Conflict("Ingredient already exists.");
        }
        db.Ingredients.Add(ingredient);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetIngredientById), new { id = ingredient.Id }, ingredient);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIngredient(int id)
    {
        Ingredient? ingredientToDelete = await db.Ingredients.FindAsync(id);
        if (ingredientToDelete != null)
        {
            db.Ingredients.Remove(ingredientToDelete);
            await db.SaveChangesAsync();
            return NoContent();
        }
        else
        {
            return NotFound();
        }
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> ChangeIngredient(int id, Ingredient ingredient)
    {
        if (!ingredient.IsValid())
        {
            return BadRequest("Invalid Input.");
        }
        if (await db.Ingredients.AnyAsync(n => n.Name == ingredient.Name && n.Id != id))
        {
            return Conflict("Ingredient already exists.");
        }
        Ingredient? ingredientToChange = await db.Ingredients.FindAsync(id);
        if (ingredientToChange != null)
        {
            ingredientToChange.Name = ingredient.Name;
            ingredientToChange.IngredientUnit = ingredient.IngredientUnit;
            await db.SaveChangesAsync();
            return Ok(ingredientToChange);
        }
        else
        {
            return NotFound();
        }
    }    
}