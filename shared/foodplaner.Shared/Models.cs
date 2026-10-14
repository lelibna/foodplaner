using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Unit
{
    Piece,
    Gram,
    Milliliter,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Mealtime
{
    Breakfast,
    Lunch,
    Dinner,
    Snack
}

public record AddRecipeIngredient(int IngredientId, decimal Amount);
public record RecipeIngredientTransfer(int IngredientId, string Name, decimal Amount, Unit Unit);
public record RecipeDetailTransfer (
    int Id,
    string Name,
    int Portions,
    string? Instructions,
    List<RecipeIngredientTransfer> Ingredients
);

public record CreateRecipeIngredientRequest(string Name, decimal Amount, Unit? unit);

public record CreateRecipeRequest
(
    string Name,
    int Portions,
    string? Instructions,
    List<CreateRecipeIngredientRequest>? ingredients
);
public record CreatePlanEntryRequest(DateOnly Date, Mealtime Mealtime, int RecipeId, int Portions)
{
    public bool IsValidFor(DateOnly start)
    {
        if(start.DayOfWeek != DayOfWeek.Monday)
        {
            return false;
        }
        if(Date < start || Date > start.AddDays(6))
        {
            return false;
        }
        if(Portions < 1 || Portions > 10)
        {
            return false;
        }
        if (!Enum.IsDefined(Mealtime))
        {
            return false;
        }
        return true;
    }
}
public record PlanEntryTransfer
(
    int Id,
    DateOnly Date,
    Mealtime Mealtime,
    int Portions,
    int RecipeId,
    string RecipeName
);

public record WeeklyPlanTransfer
(
    DateOnly StartDate,
    List<PlanEntryTransfer> PlanEntries
);