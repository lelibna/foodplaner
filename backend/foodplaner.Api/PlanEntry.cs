public class PlanEntry
{
    public int Id {get; set;}
    public int WeeklyPlanId {get; set;}
    public WeeklyPlan WeeklyPlan {get; set;} = null!;
    public int RecipeId {get; set;}
    public Recipe Recipe {get; set;} = null!;
    public DateOnly Date {get; set;}
    public Mealtime Mealtime {get; set;}
    public int Portions {get; set;}
}