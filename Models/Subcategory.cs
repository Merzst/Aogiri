namespace Aogiri.Models;

public class Subcategory
{
    public int SubcategoryID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? IconClass { get; set; } = "bi-tag";

    public int CategoryID { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();
}
