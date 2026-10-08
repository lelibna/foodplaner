public class Ingredient
{
    public int Id {get; set;}
    public string Name {get; set;} = "";
    public Unit IngredientUnit {get; set;}
    public List<RecipeIngredient> RecipeIngredients {get; set;} = new();

    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            return false;
        }
        if(!Enum.IsDefined(IngredientUnit))
        {
            return false;
        }
        return true;
    }
}

public enum Unit
{
    Piece,
    Gram,
    Milliliter,
}