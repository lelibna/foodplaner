using Microsoft.AspNetCore.Http.HttpResults;

public class WeeklyPlan
{
    public int Id {get; set;}
    public DateOnly Start {get; set;}
    public List<PlanEntry> PlanEntries {get; set;} = new();
}