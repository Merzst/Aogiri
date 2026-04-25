namespace Aogiri.Models;

public class AdImage
{
    public int    AdImageID { get; set; }
    public string ImageUrl  { get; set; } = string.Empty;
    public int    SortOrder { get; set; } = 0;   // 0 = главное фото

    public int           AdID          { get; set; }
    public Advertisement Advertisement { get; set; } = null!;
}
