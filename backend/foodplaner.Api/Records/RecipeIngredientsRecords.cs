public record AddRecipeIngredient(int IngredientId, decimal Amount);
public record RecipeIngredientTransfer(int IngredientId, string Name, decimal Amount, Unit Unit);
public record RecipeDetailTransfer (
    int Id,
    string Name,
    int Portions,
    string? Instructions,
    List<RecipeIngredientTransfer> Ingredients
);