public class Recipe
{
    public int Id {get; set;}
    public string Name {get; set;} = "";
    public int Portions {get; set;}
    public string? Instructions {get; set;}

    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            return false;
        }
        if(Portions < 1 || Portions > 10)
        {
            return false;
        }
        return true;
    }
}