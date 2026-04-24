namespace Aogiri.Models;

public class Location
{
    public int LocationID { get; set; }
    public string City { get; set; } = string.Empty;
    public string? Region { get; set; }
    public string Country { get; set; } = string.Empty;

    public ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();
}
