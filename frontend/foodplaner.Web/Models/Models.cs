public record Ingredient(int Id, string Name, Unit IngredientUnit);
public record Recipe(int Id, string Name, int Portions, String? Instructions);
public class RecipeIngredientRow
{
    public string Name {get; set;} = "";
    public decimal Amount {get; set;}
    public Unit? Unit {get; set;}

    public bool IsNew {get; set;}
}