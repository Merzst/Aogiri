namespace Aogiri.Models;

public class Category
{
    public int CategoryID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? IconClass { get; set; } = "bi-tag";

    public ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();
}
