using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Unit
{
    Piece,
    Gram,
    Milliliter,
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