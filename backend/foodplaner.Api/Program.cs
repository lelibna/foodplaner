using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/api/recipes", async (AppDbContext db) =>
{
    return await db.Recipes.ToListAsync();
});

app.MapGet("/api/recipes/{id}", async (int id, AppDbContext db) =>
{
    Recipe? recipe = await db.Recipes.FindAsync(id);
    if(recipe != null)
    {
        return Results.Ok(recipe);
    } else
    {
        return Results.NotFound();
    }
});


app.MapPost("/api/recipes", async (Recipe recipe, AppDbContext db) =>
{
    if (!recipe.IsValid())
    {
        return Results.BadRequest("Invalid Input.");
    }
    db.Recipes.Add(recipe);
    await db.SaveChangesAsync();
    return Results.Created($"/api/recipes/{recipe.Id}", recipe);
});

app.MapDelete("/api/recipes/{id}", async (int id, AppDbContext db) =>
{
    Recipe? recipeToDelete = await db.Recipes.FindAsync(id);
    if(recipeToDelete != null)
    {
        db.Recipes.Remove(recipeToDelete);
        await db.SaveChangesAsync();
        return Results.NoContent();
    } else
    {
        return Results.NotFound();
    }
});

app.MapPut("/api/recipes/{id}", async (int id, Recipe recipe, AppDbContext db) =>
{
    Recipe? recipeToChange = await db.Recipes.FindAsync(id);
    if(recipeToChange != null)
    {
        if (!recipe.IsValid())
        {
            return Results.BadRequest("Invalid Input.");
        }
        recipeToChange.Name = recipe.Name;
        recipeToChange.Instructions = recipe.Instructions;
        recipeToChange.Portions = recipe.Portions;
        await db.SaveChangesAsync();
        return Results.Ok(recipeToChange);
    } else
    {
        return Results.NotFound();
    }
});

app.Run();


