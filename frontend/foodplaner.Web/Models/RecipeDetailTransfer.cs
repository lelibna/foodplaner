public record RecipeDetailTransfer(
    int Id,
    string Name,
    int Portions,
    string? Instructions,
    List<RecipeIngredientTransfer> Ingredients
);